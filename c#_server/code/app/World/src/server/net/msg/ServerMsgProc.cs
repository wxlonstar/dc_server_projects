using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 逻辑服消息处理
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class ServerMsgProc : ConnAppProc
    {
        public ServerMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_SS2WS;
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
        }
        protected override void RegisterHandle()
        {
            RegisterMsgProc(ss2ws.msg.LOGIN_CLIENT, OnLoginAccount);
            RegisterMsgProc(ss2ws.msg.LOGOUT_CLIENT, OnLogoutAccount);
            RegisterMsgProc(ss2ws.msg.CLIENT_ONLINE, OnCheckOnline);
            RegisterMsgProc(ss2ws.msg.ONLINE_COUNT, OnPlayerCount);
            RegisterMsgProc(ss2ws.msg.UNIT_MODIFY_INT, OnUnitAttrModifyInt);
            RegisterMsgProc(ss2ws.msg.UNIT_MODIFY_STRING, OnUnitAttrModifyString);
            RegisterMsgProc(ss2ws.msg.CHAT_SEND, OnChatSend);
        }
        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }

        #region 角色
        /// <summary>
        /// 进入游戏
        /// </summary>
        private void OnLoginAccount(PacketBase packet)
        {
            ss2ws.LoginClient msg = packet as ss2ws.LoginClient;
            //加入管理器
            UnitManager.Instance.HandleLogin(msg.client_uid, msg.server_uid, msg.data);
            Log.Debug("玩家进入游戏:" + msg.data.char_idx);
        }
        /// <summary>
        /// 账号登出
        /// </summary>
        private void OnLogoutAccount(PacketBase packet)
        {
            ss2ws.LogoutClient msg = packet as ss2ws.LogoutClient;
            UnitManager.Instance.HandleLogout(msg.char_idx);
            Log.Debug("玩家离开游戏:" + msg.char_idx);
        }
        /// <summary>
        /// 检测在线
        /// </summary>
        private void OnCheckOnline(PacketBase packet)
        {
            ss2ws.ClientOnline msg = packet as ss2ws.ClientOnline;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.char_idx);
            if (unit != null)
            {
                unit.RecvCheckOnline(msg.is_online);
            }
        }       
        /// <summary>
        /// 上报玩家数量
        /// </summary>
        private void OnPlayerCount(PacketBase packet)
        {
            ss2ws.OnlineCount msg = packet as ss2ws.OnlineCount;
            ServerInfoManager.Instance.UpdatePlayerCount(msg.server_uid.ss_uid, msg.count);
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        private void OnUnitAttrModifyInt(PacketBase packet)
        {
            ss2ws.NotifyUpdatePlayerAttribInteger msg = packet as ss2ws.NotifyUpdatePlayerAttribInteger;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.char_idx);
            if (unit != null)
            {
                unit.UpdateAttribute(msg.type, msg.value);
            }
        }
        private void OnUnitAttrModifyString(PacketBase packet)
        {
            ss2ws.NotifyUpdatePlayerAttribString msg = packet as ss2ws.NotifyUpdatePlayerAttribString;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.char_idx);
            if (unit != null)
            {
                unit.UpdateAttribute(msg.type, msg.value);
            }
        }
        #endregion
        #region 聊天
        /// <summary>
        /// 发送聊天
        /// </summary>
        private void OnChatSend(PacketBase packet)
        {
            ss2ws.ChatSend msg = packet as ss2ws.ChatSend;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.sender.char_idx);
            if (unit != null)
            {
                ChatManager.Instance.HandleSendChat(unit, msg.type, msg.receiver, msg.chat_content);
            }
        }
        #endregion
    }
}
