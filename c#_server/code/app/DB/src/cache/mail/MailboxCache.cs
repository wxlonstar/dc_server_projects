using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 单个玩家的邮箱
    /// @author hannibal
    /// @time 2017-9-11
    /// </summary>
    public class MailboxCache : IPoolsObject
    {
        private long m_char_idx = 0;//邮件拥有者
        private Dictionary<long, MailInfo> m_all_mails;//所有邮件
        private List<long> m_save_mails;//需要修改标记的邮件
        private List<long> m_del_mails;//需要删除的邮件
        private MailCharRecvs m_had_recv_mails;

        private bool m_is_dirty = false;        //数据是否有更改
        private long m_last_save_time = 0;      //最后保存时间

        public MailboxCache()
        {
            m_all_mails = new Dictionary<long, MailInfo>();
            m_save_mails = new List<long>();
            m_del_mails = new List<long>();
        }
        public void Init()
        {
            m_is_dirty = false;
            m_last_save_time = Time.timeSinceStartup;
        }
        /// <summary>
        /// 读取邮箱
        /// </summary>
        public bool Setup(long _char_idx)
        {
            m_char_idx = _char_idx;
            this.LoadMails();

            return true;
        }
        public void Destroy()
        {
            this.Save();
            foreach(var obj in m_all_mails)
            {
                CommonObjectPools.Despawn(obj.Value);
            }
            m_all_mails.Clear();
            m_save_mails.Clear();
            m_del_mails.Clear();
            m_char_idx = 0;
        }
        #region 读写db
        /// <summary>
        /// 开始读取db
        /// </summary>
        private void LoadMails()
        {
            PlayerCache player = PlayerCacheManager.Instance.GetMember(m_char_idx);
            if (player == null) return;

            SQLMailHandle.LoadCharRecvs(m_char_idx, OnLoadRecvMailEnd);
            SQLMailHandle.LoadMailList(m_char_idx, player.ss_data.spid, OnLoadMailEnd);
        }
        /// <summary>
        /// 玩家领取过的全局邮件
        /// </summary>
        private void OnLoadRecvMailEnd(MailCharRecvs info)
        {
            m_had_recv_mails = info;
        }
        /// <summary>
        /// 读取列表完成
        /// </summary>
        private void OnLoadMailEnd(List<MailInfo> list)
        {
            //可能读取db时，玩家下线了
            if (m_char_idx == 0)
            {
                list.ForEach(info => CommonObjectPools.Despawn(info));
                return;
            }

            //过滤掉已经领取过附件的全局邮件
            for (int i = list.Count - 1; i >= 0; --i)
            {
                MailInfo mail_info = list[i];
                if (mail_info.receiver_idx == 0 && m_had_recv_mails.Contains(mail_info.mail_idx))
                    list.RemoveAt(i);
            }

            //保存到集合
            list.ForEach(info => m_all_mails.Add(info.mail_idx, info));

            PlayerCache player = PlayerCacheManager.Instance.GetMember(m_char_idx);
            if(player == null)
            {//可能读取db时，玩家下线了
                MailCacheManager.Instance.HanldeLogoutClient(m_char_idx);
                return;
            }

            //转发给ss，每次发10封
            while (list.Count > 0)
            {
                db2ss.MailList msg = PacketPools.Get(db2ss.msg.MAIL_LIST) as db2ss.MailList;
                msg.char_idx = m_char_idx;
                for (int i = list.Count - 1, j = 0; i >= 0 && j < 10; --i, ++j)
                {
                    msg.mail_list.Add(list[i]);
                    list.RemoveAt(i);
                }
                ForServerNetManager.Instance.Send(player.ss_uid, msg);
            }
        }

        /// <summary>
        /// 保存所有
        /// </summary>
        public void Save()
        {
            //保存需要删除的
            if (m_del_mails.Count > 0) SQLMailHandle.DeleteMail(m_del_mails);
            m_del_mails.Clear();

            //保存修改的
            if (m_save_mails.Count > 0) SQLMailHandle.ModifyMailFlags(m_all_mails, m_save_mails);

            m_is_dirty = false;
            m_last_save_time = Time.timeSinceStartup;
        }
        /// <summary>
        /// 是否需要存盘
        /// </summary>
        /// <returns></returns>
        public bool NeedSave()
        {
            if (m_is_dirty && Time.timeSinceStartup - m_last_save_time > 1000  * 5)//5分钟保存一次
                return true;
            return false;
        }
        #endregion

        #region 邮箱操作
        /// <summary>
        /// 写邮件
        /// </summary>
        public void HandleWriteMail(MailWriteInfo info)
        {
            PlayerCache player = PlayerCacheManager.Instance.GetMember(m_char_idx);
            if (player == null) return;

            if(info.receiver_type == MailWriteInfo.eReceiverType.CHARID)
            {
                ProcessWrite(info);
            }
            else if(info.receiver_type == MailWriteInfo.eReceiverType.CHARNAME)
            {
                if (String.IsNullOrEmpty(info.receiver_name))
                {
                    Log.Warning("错误的收件人name:" + info.receiver_name);
                    return;
                }
                PlayerCache recv_player = PlayerCacheManager.Instance.GetMemberByName(info.receiver_name);
                if (recv_player != null)
                {
                    info.receiver_idx = recv_player.char_idx;
                    ProcessWrite(info);
                }
                else
                {//需要查db
                    //TODO
                }
            }
            else
            {
                Log.Warning("错误的收件人type:" + info.receiver_type);
                return;
            }
        }
        private void ProcessWrite(MailWriteInfo info)
        {
            PlayerCache player = PlayerCacheManager.Instance.GetMember(m_char_idx);
            if (player == null) return;

            if (info.receiver_idx <= 0)
            {
                Log.Warning("错误的收件人id:" + info.receiver_idx);
                return;
            }
            //存db
            MailInfo mail_info = CommonObjectPools.Spawn<MailInfo>();
            mail_info.mail_type = eMailType.NORMAL;
            mail_info.spid = 0;//普通邮件，不区分渠道id
            mail_info.receiver_idx = info.receiver_idx;
            mail_info.sender_idx = m_char_idx;
            mail_info.sender_name = player.ss_data.char_name;
            mail_info.send_time = Time.second_time;
            mail_info.expire_time = 0;
            mail_info.delivery_time = 0;
            mail_info.flags = (uint)eMailFlags.NONE;
            mail_info.subject = info.subject;
            mail_info.bin_mail_content.content_type = eMailContentType.NORMAL;
            mail_info.bin_mail_content.bin_normal_content.content = info.content;
            SQLMailHandle.CreateMail(mail_info);
            CommonObjectPools.Despawn(mail_info);

            //返回结果给ss
            db2ss.MailCommand rep_msg = PacketPools.Get(db2ss.msg.MAIL_COMMAND) as db2ss.MailCommand;
            rep_msg.mail_idx = 0;
            rep_msg.command_type = eMailCommandType.WRITE_MAIL;
            rep_msg.error_type = eMailCommandError.NONE;
            ForServerNetManager.Instance.Send(player.ss_uid, rep_msg);
        }
        /// <summary>
        /// 邮件操作：优先级依次是 读->领取附件->删除
        /// </summary>
        public void HandleCommandMail(long mail_idx, eMailCommandType type)
        {
            PlayerCache player = PlayerCacheManager.Instance.GetMember(m_char_idx);
            if (player == null) return;

            switch(type)
            {
                case eMailCommandType.READ_MAIL:
                    {
                        MailInfo mail_info;
                        if (m_all_mails.TryGetValue(mail_idx, out mail_info))
                        {
                            if (m_del_mails.Contains(mail_idx)) break;
                            if (m_save_mails.Contains(mail_idx)) break;
                            if (mail_info.receiver_idx == 0)
                            {
                                if(!m_had_recv_mails.Contains(mail_idx))
                                {//同步到db，已经领取过全局邮件
                                    m_had_recv_mails.Add(mail_idx);
                                    SQLMailHandle.UpdateCharRecvs(m_char_idx, m_had_recv_mails);
                                }
                                break;
                            }

                            //判断是否已经读取
                            if (!Utils.HasFlag(mail_info.flags, (uint)eMailFlags.READED))
                            {
                                m_save_mails.Add(mail_idx);
                                mail_info.flags = Utils.InsertFlag(mail_info.flags, (uint)eMailFlags.READED);
                                m_is_dirty = true;
                            }
                        }
                    }
                    break;
                case eMailCommandType.DELETE_MAIL:
                    {
                        MailInfo mail_info;
                        if (m_all_mails.TryGetValue(mail_idx, out mail_info))
                        {
                            if (m_del_mails.Contains(mail_idx)) break;
                            if (mail_info.receiver_idx == 0) break;

                            m_del_mails.Add(mail_idx);
                            m_save_mails.Remove(mail_idx);
                            m_all_mails.Remove(mail_idx);
                            CommonObjectPools.Despawn(mail_info);
                            m_is_dirty = true;
                        }
                    }
                    break;
                case eMailCommandType.TAKE_MAIL:
                case eMailCommandType.TAKE_MAIL_BUT_RETAIN:
                    {
                        MailInfo mail_info;
                        if (m_all_mails.TryGetValue(mail_idx, out mail_info))
                        {
                            if (m_del_mails.Contains(mail_idx)) break;
                            if (mail_info.receiver_idx == 0) break;

                            //修改状态，前提是带附件
                            if (Utils.HasFlag(mail_info.flags, (uint)eMailFlags.CARRY_ITEM))
                            {
                                m_save_mails.Remove(mail_idx);
                                mail_info.flags = Utils.InsertFlag(mail_info.flags, (uint)eMailFlags.READED);
                                mail_info.flags = Utils.RemoveFlag(mail_info.flags, (uint)eMailFlags.CARRY_ITEM);
                                m_is_dirty = true;
                            }
                        }
                    }
                    break;
            }
        }
        #endregion
        public long char_idx
        {
            get { return m_char_idx; }
        }
    }
}
