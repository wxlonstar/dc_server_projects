using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            m_msg_begin_idx = ProtocolID.MSG_BASE_SS2GL;
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
            return ForServerNetManager.Instance.Send(m_conn_idx, packet);
        }
        protected override void RegisterHandle()
        {
            RegisterMsgProc(ss2gl.msg.PING_NET, OnPingNet);

            //角色
            RegisterMsgProc(ss2gl.msg.LOGIN_CLIENT, OnLoginAccount);
            RegisterMsgProc(ss2gl.msg.LOGOUT_CLIENT, OnLogoutAccount);
            RegisterMsgProc(ss2gl.msg.UNIT_MODIFY_INT, OnUnitAttrModifyInt);
            RegisterMsgProc(ss2gl.msg.UNIT_MODIFY_STRING, OnUnitAttrModifyString);

            //关系
            RegisterMsgProc(ss2gl.msg.RELATION_ADD, OnRelationAdd);
            RegisterMsgProc(ss2gl.msg.RELATION_REMOVE, OnRelationRemove);
            RegisterMsgProc(ss2gl.msg.RELATION_GIVE, OnRelationGive);
            RegisterMsgProc(ss2gl.msg.RELATION_APPLY_CMD, OnRelationApplyCommand);
        }

        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }
        /// <summary>
        /// ping网络
        /// </summary>
        private void OnPingNet(PacketBase packet)
        {
            ss2gl.PingNet msg = packet as ss2gl.PingNet;

            long offset_time = Time.time - msg.tick;
            Log.Debug("收到第:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + offset_time);

            gl2ss.PingNet rep_msg = PacketPools.Get(gl2ss.msg.PING_NET) as gl2ss.PingNet;
            rep_msg.client_uid = msg.client_uid;
            rep_msg.packet_id = msg.packet_id;
            rep_msg.tick = msg.tick;
            rep_msg.flags = msg.flags;
            ForServerNetManager.Instance.Send(msg.server_uid.ss_uid, rep_msg);
        }
        #region 角色
        private void OnLoginAccount(PacketBase packet)
        {
            ss2gl.LoginClient msg = packet as ss2gl.LoginClient;
            UnitManager.Instance.HandleLogin(msg.server_uid.ss_uid, msg.data);
            Log.Debug("玩家进入游戏:" + msg.data.char_idx);
        }
        /// <summary>
        /// 账号登出
        /// </summary>
        private void OnLogoutAccount(PacketBase packet)
        {
            ss2gl.LogoutClient msg = packet as ss2gl.LogoutClient;
            UnitManager.Instance.HandleLogout(msg.char_idx);
            Log.Debug("玩家离开游戏:" + msg.char_idx);
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        private void OnUnitAttrModifyInt(PacketBase packet)
        {
            ss2gl.NotifyUpdatePlayerAttribInteger msg = packet as ss2gl.NotifyUpdatePlayerAttribInteger;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.char_idx);
            if (unit != null)
            {
                unit.UpdateAttribute(msg.type, msg.value);
            }
        }
        private void OnUnitAttrModifyString(PacketBase packet)
        {
            ss2gl.NotifyUpdatePlayerAttribString msg = packet as ss2gl.NotifyUpdatePlayerAttribString;
            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.char_idx);
            if (unit != null)
            {
                unit.UpdateAttribute(msg.type, msg.value);
            }
        }
        #endregion
        #region 关系
        /// <summary>
        /// 添加关系
        /// </summary>
        private void OnRelationAdd(PacketBase packet)
        {
            ss2gl.RelationAdd msg = packet as ss2gl.RelationAdd;

            MemberRelation relation = RelationManager.Instance.GetMember(msg.char_idx);
            if (relation == null)
                return;

            relation.AddRelationCommand(msg.target_id, msg.flag, msg.message);
        }
        /// <summary>
        /// 移除关系
        /// </summary>
        private void OnRelationRemove(PacketBase packet)
        {
            ss2gl.RelationRemove msg = packet as ss2gl.RelationRemove;

            MemberRelation relation = RelationManager.Instance.GetMember(msg.char_idx);
            if (relation == null)
                return;

            relation.RemoveRelationCommand(msg.target_char_idx);
        }
        /// <summary>
        /// 赠送
        /// </summary>
        private void OnRelationGive(PacketBase packet)
        {
            ss2gl.RelationGive msg = packet as ss2gl.RelationGive;

            MemberRelation relation = RelationManager.Instance.GetMember(msg.char_idx);
            if (relation == null)
                return;

            relation.FriendGiveCommand(msg.target_char_idx, msg.item_id);
        }
        /// <summary>
        /// 操作
        /// </summary>
        private void OnRelationApplyCommand(PacketBase packet)
        {
            ss2gl.RelationApplyCmd msg = packet as ss2gl.RelationApplyCmd;

            MemberRelation relation = RelationManager.Instance.GetMember(msg.char_idx);
            if (relation != null)
            {
                relation.ApplyRelationCommand(msg.event_idx, msg.target_char_idx, msg.cmd);
            }
        }
        #endregion
    }
}
