using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 单个玩家的邮箱
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public class Mailbox : IPoolsObject
    {
        private long m_char_idx;//邮件拥有者

        private List<long> m_new_mails = null;//收到的新邮件
        private List<long> m_del_mails;//需要删除的邮件
        private List<long> m_save_mails;//需要修改标记的邮件
        private MailCharRecvs m_had_recv_mails;//已经接收的全局邮件
        private Dictionary<long, MailInfo> m_all_mails = null;//玩家当前拥有的邮件

        private bool m_is_dirty = false;        //数据是否有更改
        private long m_last_save_time = 0;      //最后保存时间

        public Mailbox()
        {
            m_new_mails = new List<long>();
            m_del_mails = new List<long>();
            m_save_mails = new List<long>();
            m_all_mails = new Dictionary<long, MailInfo>();
        }
        public void Init()
        {
            m_is_dirty = false;
            m_last_save_time = Time.timeSinceStartup;
        }
        public void Setup(long _char_idx)
        {
            m_char_idx = _char_idx;
            this.LoadMail();
        }
        public void Destroy()
        {
            foreach (var obj in m_all_mails)
            {
                CommonObjectPools.Despawn(obj.Value);
            }
            this.Save();
            m_all_mails.Clear();
            m_new_mails.Clear();
            m_save_mails.Clear();
            m_del_mails.Clear();
            m_char_idx = 0;
        }
        #region 读写邮件
        private void LoadMail()
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null) return;

            SQLMailHandle.LoadCharRecvs(m_char_idx, player.db_id, (info) =>
            {
                m_had_recv_mails = info;
                SQLMailHandle.LoadMailList(m_char_idx, player.spid, player.db_id, OnLoadMailEnd);
            });
        }
        /// <summary>
        /// 投递邮件到信箱
        /// </summary>
        private void OnLoadMailEnd(List<MailInfo> list)
        {
            if (list.Count == 0) return;

            //可能读取db时，玩家下线了
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
            {
                list.ForEach(info => CommonObjectPools.Despawn(info));
                return;
            }

            //过滤掉已经领取过附件的全局邮件
            for (int i = list.Count - 1; i >= 0; --i)
            {
                MailInfo mail_info = list[i];
                if (mail_info.receiver_idx == 0 && m_had_recv_mails.Contains(mail_info.mail_idx))
                {
                    CommonObjectPools.Despawn(mail_info);
                    list.RemoveAt(i);
                }
            }

            //添加到邮件集合
            list.ForEach(info => { if (!m_all_mails.ContainsKey(info.mail_idx)) m_all_mails.Add(info.mail_idx, info); });

            //加入到新邮件列表
            list.ForEach(info => m_new_mails.Add(info.mail_idx));

            //过滤掉系统内部邮件
            List<long> del_list = new List<long>();
            for(int i = list.Count - 1; i>= 0; i--)
            {
                MailInfo mail_info = list[i];
                if (mail_info.mail_type == eMailType.SYSTEM_INTERNAL)
                {
                    if (mail_info.bin_mail_content.content_type == eMailContentType.MOD)
                    {//修改玩家属性
                        MailModContent content = mail_info.bin_mail_content.bin_mod_content;
                        foreach(var obj in content.mods)
                        {
                            player.unit_attr.SetAttrib(obj.type, obj.value_i, obj.value_str);
                        }
                        list.RemoveAt(i);
                        del_list.Add(mail_info.mail_idx);
                    }
                }
            }
            if (del_list.Count > 0) this.HandleDeleteMail(del_list);

            //通知客户端有新邮件到达
            if(list.Count > 0)
            {
                //获取未读邮件数量
                int unread_count = 0;
                list.ForEach(info => { if (!Utils.HasFlag(info.flags, (uint)eMailFlags.READED))unread_count++; });

                ss2c.MailCount msg = PacketPools.Get(ss2c.msg.MAIL_COUNT) as ss2c.MailCount;
                msg.total_mail_count = (ushort)list.Count;
                msg.unread_mail_count = (ushort)unread_count;
                ServerNetManager.Instance.SendProxy(player.client_uid, msg);
            }
        }
        /// <summary>
        /// 保存所有
        /// </summary>
        public void Save()
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
            {
                return;
            }

            //保存需要删除的
            if (m_del_mails.Count > 0)
            {
                SQLMailHandle.DeleteMail(m_del_mails, player.db_id);
                m_del_mails.Clear();
            }
            //保存修改的
            if (m_save_mails.Count > 0)
            {
                SQLMailHandle.ModifyMailFlags(m_all_mails, m_save_mails, player.db_id);
                m_save_mails.Clear();
            }

            m_is_dirty = false;
            m_last_save_time = Time.timeSinceStartup;
        }
        /// <summary>
        /// 是否需要存盘
        /// </summary>
        /// <returns></returns>
        public bool NeedSave()
        {
            if (m_is_dirty && Time.timeSinceStartup - m_last_save_time > 1000 * 5)//5分钟保存一次
                return true;
            return false;
        }
        #endregion
        /// <summary>
        /// 前端请求邮件列表
        /// </summary>
        public void HandleReqList()
        {            
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
            {
                return;
            }
            if(m_new_mails.Count > 0)
            {
                ss2c.MailList msg = PacketPools.Get(ss2c.msg.MAIL_LIST) as ss2c.MailList;
                foreach (var mail_idx in m_new_mails)
                {
                    MailInfo mail_info;
                    if (m_all_mails.TryGetValue(mail_idx, out mail_info))
                    {
                        MailTitleInfo info = new MailTitleInfo();
                        info.CopyFromInfo(mail_info);
                        msg.mail_list.Add(info);
                    }
                }
                ServerNetManager.Instance.SendProxy(player.client_uid, msg);
                m_new_mails.Clear();
            }
        }
        /// <summary>
        /// 读邮件
        /// </summary>
        public void HandleReadMail(long mail_idx)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
            {
                return;
            }
            MailInfo mail_info;
            if(m_all_mails.TryGetValue(mail_idx, out mail_info))
            {
                if (m_del_mails.Contains(mail_idx)) return;

                //是否全服邮件
                if (mail_info.receiver_idx == 0)
                {
                    if (!m_had_recv_mails.Contains(mail_idx))
                    {//同步到db，已经领取过全局邮件
                        m_had_recv_mails.Add(mail_idx);
                        SQLMailHandle.UpdateCharRecvs(m_char_idx, m_had_recv_mails, player.db_id);
                    }
                }
                else if (!Utils.HasFlag(mail_info.flags, (uint)eMailFlags.READED))
                {//修改标记
                    mail_info.flags = Utils.InsertFlag(mail_info.flags, (uint)eMailFlags.READED);
                    m_save_mails.Add(mail_idx);
                    m_is_dirty = true;
                }

                //通知客户端
                ss2c.MailRead msg_client = PacketPools.Get(ss2c.msg.MAIL_READ) as ss2c.MailRead;
                msg_client.mail_info.Copy(mail_info);
                ServerNetManager.Instance.SendProxy(player.client_uid, msg_client);
            }
        }
        /// <summary>
        /// 写邮件
        /// </summary>
        public void HandleWriteMail(MailWriteInfo info)
        {
            if (info.receiver.IsIdxValid())
            {
                ProcessWrite(info);
            }
            else
            {
                if (String.IsNullOrEmpty(info.receiver.char_name))
                {
                    Log.Warning("错误的收件人name:" + info.receiver.char_name);
                    return;
                }
                Player recv_player = UnitManager.Instance.GetPlayerByName(info.receiver.char_name);
                if (recv_player != null)
                {
                    info.receiver.char_idx = recv_player.char_idx;
                    ProcessWrite(info);
                }
                else
                {//需要查db
                    Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
                    if (player == null)
                        return;
                    SQLCharHandle.QueryCharacterInfoByName(info.receiver.char_name, player.db_id, (ret, data) =>
                    {
                        if(ret && m_char_idx > 0)
                        {
                            info.receiver.char_idx = data.char_idx;
                            ProcessWrite(info);
                        }
                    });
                }
            }
        }
        private void ProcessWrite(MailWriteInfo info)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
            {
                return;
            }

            if (info.receiver.char_idx == 0)
            {
                Log.Warning("错误的收件人id:" + info.receiver.char_idx);
                return;
            }

            //存db
            MailInfo mail_info = CommonObjectPools.Spawn<MailInfo>();
            mail_info.mail_type = eMailType.NORMAL;
            mail_info.spid = 0;//普通邮件，不区分渠道id
            mail_info.receiver_idx = info.receiver.char_idx;
            mail_info.sender_idx = m_char_idx;
            mail_info.sender_name = player.char_name;
            mail_info.send_time = Time.second_time;
            mail_info.expire_time = 0;
            mail_info.delivery_time = 0;
            mail_info.flags = (uint)eMailFlags.NONE;
            mail_info.subject = info.subject;
            mail_info.bin_mail_content.content_type = eMailContentType.NORMAL;
            mail_info.bin_mail_content.bin_normal_content.content = info.content;
            SQLMailHandle.CreateMail(mail_info, player.db_id);
            CommonObjectPools.Despawn(mail_info);
        }
        /// <summary>
        /// 领取附件
        /// </summary>
        public void HandleTakeMail(long mail_idx, bool delete)
        {
            if (m_del_mails.Contains(mail_idx)) return;

            MailInfo mail_info;
            if (m_all_mails.TryGetValue(mail_idx, out mail_info))
            {
                if (mail_info.receiver_idx == 0) return;

                //修改状态，前提是带附件
                if (Utils.HasFlag(mail_info.flags, (uint)eMailFlags.CARRY_ITEM))
                {
                    m_save_mails.Add(mail_idx);
                    mail_info.flags = Utils.InsertFlag(mail_info.flags, (uint)eMailFlags.READED);
                    mail_info.flags = Utils.RemoveFlag(mail_info.flags, (uint)eMailFlags.CARRY_ITEM);
                    m_is_dirty = true;
                }
            }
        }
        /// <summary>
        /// 删除邮件
        /// </summary>
        /// <param name="list">需要删除的列表</param>
        public void HandleDeleteMail(List<long> list)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
            {
                return;
            }
            foreach(var mail_idx in list)
            {
                MailInfo mail_info;
                if (!m_all_mails.TryGetValue(mail_idx, out mail_info)) continue;

                if (m_del_mails.Contains(mail_idx)) continue;
                if (mail_info.receiver_idx == 0) continue;

                // 告诉客户端
                if (mail_info.mail_type != eMailType.SYSTEM_INTERNAL)
                {
                    ss2c.MailCommand msg_client = PacketPools.Get(ss2c.msg.MAIL_COMMAND) as ss2c.MailCommand;
                    msg_client.mail_idx = mail_idx;
                    msg_client.command_type = eMailCommandType.DELETE_MAIL;
                    msg_client.error_type = eMailCommandError.NONE;
                    ServerNetManager.Instance.SendProxy(player.client_uid, msg_client);
                }

                // 修改邮件集合
                m_del_mails.Add(mail_idx);
                m_new_mails.Remove(mail_idx);
                m_save_mails.Remove(mail_idx);
                m_all_mails.Remove(mail_idx);
                CommonObjectPools.Despawn(mail_info);
            }
        }
        public long char_idx
        {
            get { return m_char_idx; }
        }
    }
}
