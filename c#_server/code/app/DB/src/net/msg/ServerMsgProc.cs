using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 逻辑服消息处理
    /// @author hannibal
    /// @time 2017-8-16
    /// </summary>
    public class ServerMsgProc : ConnAppProc
    {
        public ServerMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_SS2DB;
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
        }
        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public override int Send(PacketBase packet)
        {
            if (packet is PackBaseS2S)
                ((PackBaseS2S)packet).server_uid.db_uid = ServerNetManager.Instance.srv_uid;

            return ForServerNetManager.Instance.Send(m_conn_idx, packet);
        }
        protected override void RegisterHandle()
        {
            RegisterMsgProc(ss2db.msg.ENUM_CHAR, OnEnumCharacter);
            RegisterMsgProc(ss2db.msg.CREATE_CHARACTER, OnCreateCharacter);
            RegisterMsgProc(ss2db.msg.CHARACTER_INFO, OnCharacterInfo);
            RegisterMsgProc(ss2db.msg.LOGOUT_CLIENT, OnLogoutClient);
            RegisterMsgProc(ss2db.msg.KICK_ACCOUNT, OnKickoutAccount);
            RegisterMsgProc(ss2db.msg.UNIT_MODIFY_INT, OnUnitAttrModifyInt);
            RegisterMsgProc(ss2db.msg.UNIT_MODIFY_STRING, OnUnitAttrModifyString);
            RegisterMsgProc(ss2db.msg.MAIL_WRITE, OnMailWrite);
            RegisterMsgProc(ss2db.msg.MAIL_COMMAND, OnMailCommand);
        }

        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }
        #region 角色
        /// <summary>
        /// 角色列表
        /// </summary>
        private void OnEnumCharacter(PacketBase packet)
        {
            ss2db.EnumCharacter msg = packet as ss2db.EnumCharacter;

            InterServerID server_uid = msg.server_uid;
            ClientUID client_uid = msg.client_uid;
            SQLCharHandle.QueryCharacterList(msg.account_idx, (data) =>
            {
                db2ss.EnumCharacter rep_msg = PacketPools.Get(db2ss.msg.ENUM_CHAR) as db2ss.EnumCharacter;
                rep_msg.server_uid = server_uid;
                rep_msg.client_uid = client_uid;
                foreach (var char_data in data)
                {
                    rep_msg.list.Add(char_data);
                }
                rep_msg.result = eEnumCharResult.E_SUCCESS;
                this.Send(rep_msg);
            }
            );
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        private void OnCreateCharacter(PacketBase packet)
        {
            ss2db.CreateCharacter msg = packet as ss2db.CreateCharacter;

            InterServerID server_uid = msg.server_uid;
            ClientUID client_uid = msg.client_uid;
            CreateCharacterInfo create_info = msg.data;
            create_info.char_idx = ServerNetManager.Instance.GetNextCharIdx();

            SQLCharHandle.CreateCharacter(msg.account_idx, create_info, (res) =>
            {
                eCreateCharResult result = eCreateCharResult.E_FAILED_COMMONERROR;
                if (create_info.char_idx == res)
                    result = eCreateCharResult.E_SUCCESS;
                else
                {
                    switch (res)
                    {
                        case 0: result = eCreateCharResult.E_FAILED_INTERNALERROR; break;
                        case 1: result = eCreateCharResult.E_FAILED_CHARCOUNTLIMIT; break;
                        case 2: result = eCreateCharResult.E_FAILED_INVALIDPARAM_REPEATEDNAME; break;
                        case 3: result = eCreateCharResult.E_FAILED_COMMONERROR; break;
                    }
                }

                db2ss.CreateCharacter rep_msg = PacketPools.Get(db2ss.msg.CREATE_CHARACTER) as db2ss.CreateCharacter;
                rep_msg.server_uid = server_uid;
                rep_msg.client_uid = client_uid;
                rep_msg.char_idx = create_info.char_idx;
                rep_msg.char_name = create_info.char_name;
                rep_msg.result = result;
                this.Send(rep_msg);
            }
            );
        }
        private void OnCharacterInfo(PacketBase packet)
        {
            ss2db.CharacterInfo msg = packet as ss2db.CharacterInfo;

            PlayerCache member = PlayerCacheManager.Instance.GetMember(msg.char_idx);
            if(member == null)
            {
                //由于数据包会放入对象池，所以在这先保存临时数据
                ClientUID client_uid = msg.client_uid; InterServerID server_uid = msg.server_uid; long char_idx = msg.char_idx;
                PlayerCacheManager.Instance.LoadPlayer(msg.char_idx, idx =>
                {
                    OnCharacterInfoLoaded(client_uid, server_uid, char_idx);
                });
            }
            else
            {
                OnCharacterInfoLoaded(msg.client_uid, msg.server_uid, msg.char_idx);
            }
        }
        private void OnCharacterInfoLoaded(ClientUID client_uid, InterServerID server_uid, long char_idx)
        {            
            PlayerCache member = PlayerCacheManager.Instance.GetMember(char_idx);
            if (member == null)
            {
                Log.Warning("OnPlayerInfoLoaded - 未找到玩家缓存信息:" + char_idx);
                return;
            }
            //设置服务器信息
            member.client_uid = client_uid;
            member.ss_uid = server_uid.ss_uid;
            member.UpdateAttribute(eUnitModType.UMT_time_last_login, Time.second_time);

            //返回数据给ss
            db2ss.CharacterInfo rep_msg = PacketPools.Get(db2ss.msg.CHARACTER_INFO) as db2ss.CharacterInfo;
            rep_msg.server_uid = server_uid;
            rep_msg.client_uid = client_uid;
            rep_msg.data.Copy(member.ss_data);
            this.Send(rep_msg);

            //读取邮件
            MailCacheManager.Instance.LoadMailbox(member.char_idx);
        }
        private void OnLogoutClient(PacketBase packet)
        {
            ss2db.LogoutClient msg = packet as ss2db.LogoutClient;
            PlayerCacheManager.Instance.HanldeLogoutClient(msg.char_idx);
        }
        private void OnKickoutAccount(PacketBase packet)
        {
            ss2db.KickoutAccount msg = packet as ss2db.KickoutAccount;
            PlayerCacheManager.Instance.HanldeLogoutClient(msg.char_idx);
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        private void OnUnitAttrModifyInt(PacketBase packet)
        {
            ss2db.NotifyUpdatePlayerAttribInteger msg = packet as ss2db.NotifyUpdatePlayerAttribInteger;

            PlayerCache member = PlayerCacheManager.Instance.GetMember(msg.char_idx);
            if(member != null)
            {
                member.UpdateAttribute(msg.type, msg.value);
            }
            else
            {
                Log.Warning("OnUnitAttrModifyInt - 未找到玩家缓存信息:" + msg.char_idx);
            }
        }
        private void OnUnitAttrModifyString(PacketBase packet)
        {
            ss2db.NotifyUpdatePlayerAttribString msg = packet as ss2db.NotifyUpdatePlayerAttribString;

            PlayerCache member = PlayerCacheManager.Instance.GetMember(msg.char_idx);
            if (member != null)
            {
                member.UpdateAttribute(msg.type, msg.value);
            }
            else
            {
                Log.Warning("OnUnitAttrModifyString - 未找到玩家缓存信息:" + msg.char_idx);
            }
        }
        #endregion
        #region 邮件
        /// <summary>
        /// 写邮件
        /// </summary>
        private void OnMailWrite(PacketBase packet)
        {
            ss2db.MailWrite msg = packet as ss2db.MailWrite;

            //判断接收者是否正确

            MailboxCache mail_box = MailCacheManager.Instance.GetMailbox(msg.char_idx);
            if(mail_box != null)
            {
                mail_box.HandleWriteMail(msg.mail_info);
            }
        }
        /// <summary>
        /// 邮件操作
        /// </summary>
        private void OnMailCommand(PacketBase packet)
        {
            ss2db.MailCommand msg = packet as ss2db.MailCommand;

            MailboxCache mail_box = MailCacheManager.Instance.GetMailbox(msg.char_idx);
            if (mail_box != null)
            {
                mail_box.HandleCommandMail(msg.mail_idx, msg.command_type);
                //mail_box.HandleCommandMail(31, msg.command_type);
                //mail_box.HandleCommandMail(32, msg.command_type);
            }
        }
        #endregion
    }
}
