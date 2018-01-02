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
            m_msg_begin_idx = ProtocolID.MSG_BASE_SS2FS;
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
        public override int Send(PacketBase packet)
        {
            return ForServerNetManager.Instance.Send(m_conn_idx, packet);
        }
        protected override void RegisterHandle()
        {
            RegisterMsgProc(ss2fs.msg.LOGIN_CLIENT, OnLoginAccount);
            RegisterMsgProc(ss2fs.msg.LOGOUT_CLIENT, OnLogoutAccount);
            RegisterMsgProc(ss2fs.msg.CLIENT_ONLINE, OnCheckOnline);
            RegisterMsgProc(ss2fs.msg.UNIT_MODIFY_INT, OnUnitAttrModifyInt);
            RegisterMsgProc(ss2fs.msg.UNIT_MODIFY_STRING, OnUnitAttrModifyString);
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
            ss2fs.LoginClient msg = packet as ss2fs.LoginClient;
            UnitManager.Instance.HandleLogin(msg.client_uid, msg.server_uid, msg.data);
            Log.Debug("玩家进入游戏:" + msg.data.char_idx);
        }
        /// <summary>
        /// 账号登出
        /// </summary>
        private void OnLogoutAccount(PacketBase packet)
        {
            ss2fs.LogoutClient msg = packet as ss2fs.LogoutClient;
            UnitManager.Instance.HandleLogout(msg.char_idx);
            Log.Debug("玩家离开游戏:" + msg.char_idx);
        }
        /// <summary>
        /// 检测在线
        /// </summary>
        private void OnCheckOnline(PacketBase packet)
        {
            ss2fs.ClientOnline msg = packet as ss2fs.ClientOnline;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.char_idx);
            if (unit != null)
            {
                unit.RecvCheckOnline(msg.is_online);
            }
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        private void OnUnitAttrModifyInt(PacketBase packet)
        {
            ss2fs.NotifyUpdatePlayerAttribInteger msg = packet as ss2fs.NotifyUpdatePlayerAttribInteger;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.char_idx);
            if (unit != null)
            {
                unit.UpdateAttribute(msg.type, msg.value);
            }
        }
        private void OnUnitAttrModifyString(PacketBase packet)
        {
            ss2fs.NotifyUpdatePlayerAttribString msg = packet as ss2fs.NotifyUpdatePlayerAttribString;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.char_idx);
            if (unit != null)
            {
                unit.UpdateAttribute(msg.type, msg.value);
            }
        }
        #endregion
    }
}
