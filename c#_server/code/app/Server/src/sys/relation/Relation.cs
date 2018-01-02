using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 单个角色关系
    /// @author hannibal
    /// @time 2016-9-27
    /// </summary>
    public class MemberRelation : IPoolsObject
    {
        private long m_char_idx = 0;
        private Dictionary<long, RelationInfo> m_relations = null;
        private Dictionary<RelationAddTarget, long> m_add_relations = null;

        public MemberRelation()
        {
            m_relations = new Dictionary<long, RelationInfo>();
            m_add_relations = new Dictionary<RelationAddTarget, long>();
        }

        /// <summary>
        /// 对象池初始化
        /// </summary>
        public void Init()
        {
            m_char_idx = 0;
        }
        /// <summary>
        /// 构建
        /// </summary>
        public void Setup(long _char_idx)
        {
            m_char_idx = _char_idx;
        }
        public void Destroy()
        {
            foreach (var obj in m_relations)
            {
                CommonObjectPools.Despawn(obj.Value);
            }
            m_relations.Clear();
        }
        public void Update()
        {
            //移除过期请求
            foreach(var obj in m_add_relations)
            {
                if(Time.timeSinceStartup - obj.Value >= relation.ADD_RELATION_TIME_OFFSET * 1000)
                {
                    m_add_relations.Remove(obj.Key);
                    break;
                }
            }
        }
        public long char_idx
        {
            get { return m_char_idx; }
        }

        #region 集合管理
        public void AddRelation(RelationInfo info)
        {
            if (m_relations.ContainsKey(info.char_idx))
                return;
            m_relations.Add(info.char_idx, info);
        }
        public void RemoveRelation(long char_idx)
        {
            RelationInfo info;
            if (!m_relations.TryGetValue(char_idx, out info))
                return;
            CommonObjectPools.Despawn(info);
            m_relations.Remove(char_idx);
        }
        public RelationInfo GetRelation(long char_idx)
        {
            RelationInfo info;
            if (!m_relations.TryGetValue(char_idx, out info))
                return null;
            return info;
        }
        /// <summary>
        /// 超出关系上限
        /// </summary>
        public bool IsRelationFull(eRelationFlag flag)
        {
            int count = 0;
            foreach (var obj in m_relations)
            {
                if (obj.Value.flags == flag) count++;
            }
            switch (flag)
            {
                case eRelationFlag.Friend:
                    if (count >= relation.PRIVATE_MAX_FRIEND_COUNT)
                        return true;
                    break;
                case eRelationFlag.Block:
                    if (count >= relation.PRIVATE_MAX_BLOCK_COUNT)
                        return true;
                    break;
            }
            return false;
        }
        /// <summary>
        /// 和对方是否存在关系
        /// </summary>
        public bool HaveRelationFlag(long target_char_idx, eRelationFlag flag)
        {
            RelationInfo info = this.GetRelation(char_idx);
            if (info != null && Utils.HasFlag((uint)info.flags, (uint)flag))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 处理client消息
        /// <summary>
        /// 添加关系
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="flag">关系标记</param>
        public void AddRelationClient(RelationAddTarget target, eRelationFlag flag, string message)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
                return;

            //超过上限
            if (IsRelationFull(flag))
                return;

            //判断是否已经存在关系
            if (target.type == eRelationAddType.Idx)
            {
                RelationInfo relation_info;
                if (m_relations.TryGetValue(target.char_idx, out relation_info))
                {
                    //已经存在相同关系，或黑名单状态加好友
                    if (relation_info.flags == flag || (relation_info.flags == eRelationFlag.Block && flag == eRelationFlag.Friend))
                        return;
                }
            }
            else
            {
                foreach (var relation_info in m_relations)
                {
                    if (relation_info.Value.char_name == target.char_name)
                    {
                        //已经存在相同关系，或黑名单状态加好友
                        if (relation_info.Value.flags == flag || (relation_info.Value.flags == eRelationFlag.Block && flag == eRelationFlag.Friend))
                            return;
                    }
                }
            }

            //是否已经请求过
            long last_add_time = 0;
            if (m_add_relations.TryGetValue(target, out last_add_time))
            {
                if (Time.timeSinceStartup - last_add_time < relation.ADD_RELATION_TIME_OFFSET * 1000)
                    return;
                m_add_relations.Remove(target);
            }

            //发送到gl
            ss2gl.RelationAdd msg = PacketPools.Get(ss2gl.msg.RELATION_ADD) as ss2gl.RelationAdd;
            msg.char_idx = m_char_idx;
            msg.target_id = target;
            msg.flag = flag;
            msg.message = message;
            ServerNetManager.Instance.Send2GL(msg);

            //添加到请求队列
            m_add_relations.Add(target, Time.timeSinceStartup);
        }
        public void RemoveRelationClient(long target_char_idx)
        {
            if (!m_relations.ContainsKey(target_char_idx)) return;

            //发送到gl
            ss2gl.RelationRemove msg = PacketPools.Get(ss2gl.msg.RELATION_REMOVE) as ss2gl.RelationRemove;
            msg.char_idx = m_char_idx;
            msg.target_char_idx = target_char_idx;
            ServerNetManager.Instance.Send2GL(msg);
        }
        /// <summary>
        /// 申请反馈
        /// </summary>
        public void ApplyCommandClient(long event_idx, long target_char_idx, eRelationApplyCmd cmd)
        {
            //发送到gl
            ss2gl.RelationApplyCmd msg = PacketPools.Get(ss2gl.msg.RELATION_APPLY_CMD) as ss2gl.RelationApplyCmd;
            msg.event_idx = event_idx;
            msg.char_idx = m_char_idx;
            msg.target_char_idx = target_char_idx;
            msg.cmd = cmd;
            ServerNetManager.Instance.Send2GL(msg);
        }
        /// <summary>
        /// 下发关系列表
        /// </summary>
        public void RelationListClient()
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
            {
                return;
            }
            List<RelationInfo> list = new List<RelationInfo>();
            foreach(var obj in m_relations)
            {
                list.Add(obj.Value);
                if (list.Count == 10)
                {//每次下发10个
                    ss2c.RelationList msg = PacketPools.Get(ss2c.msg.RELATION_LIST) as ss2c.RelationList;
                    msg.list.AddRange(list);
                    ServerNetManager.Instance.SendProxy(player.client_uid, msg, false);

                    list.Clear();
                }
            }
            if(list.Count > 0)
            {
                ss2c.RelationList msg = PacketPools.Get(ss2c.msg.RELATION_LIST) as ss2c.RelationList;
                msg.list.AddRange(list);
                ServerNetManager.Instance.SendProxy(player.client_uid, msg, false);
            }
        }
        /// <summary>
        /// 好友赠送
        /// </summary>
        public void FriendGiveClient(long target_char_idx, ItemID item_id)
        {
            //先查找是否有好友
            RelationInfo relation_info;
            if (!m_relations.TryGetValue(target_char_idx, out relation_info) || relation_info.flags != eRelationFlag.Friend)
                return;

            //是否今日已经赠送
            //TODO

            //赠送是否合法
            if (!item.IsValidItem(item_id))
                return;

            //发送到gl
            ss2gl.RelationGive msg = PacketPools.Get(ss2gl.msg.RELATION_GIVE) as ss2gl.RelationGive;
            msg.char_idx = m_char_idx;
            msg.target_char_idx = target_char_idx;
            msg.item_id = item_id;
            ServerNetManager.Instance.Send2GL(msg);
        }
        #endregion

        #region 处理gl消息
        /// <summary>
        /// 添加关系：发给对方验证
        /// </summary>
        public void AddRelationGL(long event_idx, PlayerIDName player_id, eRelationFlag flag, string message)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
                return;

            //超过上限
            if (IsRelationFull(flag))
                return;

            //判断是否已经存在关系
            RelationInfo relation_info;
            if (m_relations.TryGetValue(player_id.char_idx, out relation_info) && relation_info.flags == flag)
                return;

            //发给client处理是否同意
            if (flag != eRelationFlag.Block)
            {
                ss2c.RelationAdd rep_msg = PacketPools.Get(ss2c.msg.RELATION_ADD) as ss2c.RelationAdd;
                rep_msg.event_idx = event_idx;
                rep_msg.player_id = player_id;
                rep_msg.message = message;
                rep_msg.flag = flag;
                ServerNetManager.Instance.SendProxy(player.client_uid, rep_msg);
            }
        }
        /// <summary>
        /// 移除关系
        /// </summary>
        public void RemoveRelationGL(long target_char_idx)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
                return;

            if (!m_relations.ContainsKey(target_char_idx)) return;

            this.RemoveRelation(target_char_idx);

            ss2c.RelationRemove rep_msg = PacketPools.Get(ss2c.msg.RELATION_REMOVE) as ss2c.RelationRemove;
            rep_msg.target_char_idx = target_char_idx;
            ServerNetManager.Instance.SendProxy(player.client_uid, rep_msg);
        }
        /// <summary>
        /// 赠送
        /// </summary>
        public void FriendGiveGL(PlayerIDName src_player_id, ItemID item_id)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null)
                return;

            if (!m_relations.ContainsKey(src_player_id.char_idx)) return;

            ss2c.RelationGive rep_msg = PacketPools.Get(ss2c.msg.RELATION_GIVE) as ss2c.RelationGive;
            rep_msg.player_id = src_player_id;
            rep_msg.item_id = item_id;
            ServerNetManager.Instance.SendProxy(player.client_uid, rep_msg);
        }
        /// <summary>
        /// gl下发的关系列表
        /// </summary>
        public void RelationListGL(RelationInfo info)
        {
            RelationInfo relation_info;
            if (m_relations.TryGetValue(info.char_idx, out relation_info))
            {
                relation_info.Copy(info);
            }
            else
            {
                relation_info = CommonObjectPools.Spawn<RelationInfo>();
                relation_info.Copy(info);
                this.AddRelation(relation_info);
            }
        }
        #endregion

        #region 属性改变，同步信息
        /// <summary>
        /// 属性改变
        /// </summary>
        public void UpdateAttribute(long char_idx, eUnitModType type, long value)
        {
            RelationInfo info = GetRelation(char_idx);
            if (info == null) return;

            switch (type)
            {
                case eUnitModType.UMT_char_type: info.char_type = (byte)value; break;
                case eUnitModType.UMT_model_idx: info.model_idx = (uint)value; break;
                case eUnitModType.UMT_job: info.job = (byte)value; break;
                case eUnitModType.UMT_level: info.level = (ushort)value; break;
                case eUnitModType.UMT_exp: info.exp = (uint)value; break;
                case eUnitModType.UMT_gold: info.gold = (uint)value; break;
                case eUnitModType.UMT_coin: info.coin = (uint)value; break;
                case eUnitModType.UMT_vip_grade: info.vip_grade = (uint)value; break;
                default: return;
            }
        }
        public void UpdateAttribute(long char_idx, eUnitModType type, string value)
        {
            RelationInfo info = GetRelation(char_idx);
            if (info == null) return;

            switch (type)
            {
                case eUnitModType.UMT_char_name: info.char_name = value; break;
                default: return;
            }
        }
        #endregion
    }
}
