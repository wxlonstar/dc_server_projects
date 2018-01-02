using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 全局服消息处理
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class GlobalMsgProc : ConnAppProc
    {
        public GlobalMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_GL2SS;
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
            RegisterMsgProc(gl2ss.msg.PING_NET, OnPingNet);
            //关系
            RegisterMsgProc(gl2ss.msg.RELATION_ADD, OnRelationAdd);
            RegisterMsgProc(gl2ss.msg.RELATION_REMOVE, OnRelationRemove);
            RegisterMsgProc(gl2ss.msg.RELATION_GIVE, OnRelationGive);
            RegisterMsgProc(gl2ss.msg.RELATION_LIST, OnRelationList);
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
            gl2ss.PingNet msg = packet as gl2ss.PingNet;

            ss2c.PingNet rep_msg = PacketPools.Get(ss2c.msg.PING_NET) as ss2c.PingNet;
            rep_msg.packet_id = msg.packet_id;
            rep_msg.tick = msg.tick;
            rep_msg.flags = msg.flags;
            ServerNetManager.Instance.SendProxy(msg.client_uid, rep_msg);
        }

        #region 关系
        /// <summary>
        /// 添加关系
        /// </summary>
        private void OnRelationAdd(PacketBase packet)
        {
            gl2ss.RelationAdd msg = packet as gl2ss.RelationAdd;

            MemberRelation relation = RelationManager.Instance.GetMember(msg.char_idx);
            if (relation != null)
            {
                relation.AddRelationGL(msg.event_idx, msg.player_id, msg.flag, msg.message);
            }
        }
        /// <summary>
        /// 移除关系
        /// </summary>
        private void OnRelationRemove(PacketBase packet)
        {
            gl2ss.RelationRemove msg = packet as gl2ss.RelationRemove;

            MemberRelation relation = RelationManager.Instance.GetMember(msg.char_idx);
            if (relation != null)
            {
                relation.RemoveRelationGL(msg.target_idx);
            }
        }
        /// <summary>
        /// 赠送
        /// </summary>
        private void OnRelationGive(PacketBase packet)
        {
            gl2ss.RelationGive msg = packet as gl2ss.RelationGive;

            MemberRelation relation = RelationManager.Instance.GetMember(msg.char_idx);
            if (relation != null)
            {
                relation.FriendGiveGL(msg.src_player_id, msg.item_id);
            }
        }
        /// <summary>
        /// 列表
        /// </summary>
        private void OnRelationList(PacketBase packet)
        {
            gl2ss.RelationList msg = packet as gl2ss.RelationList;

            MemberRelation relation = RelationManager.Instance.GetMember(msg.char_idx);
            if(relation != null)
            {
                relation.RelationListGL(msg.relation_info);
            }
        }
        #endregion
    }
}
