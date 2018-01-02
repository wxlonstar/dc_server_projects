using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 角色关系
    /// @author hannibal
    /// @time 2016-9-27
    /// </summary>
    public class MemberRelation
    {
        private long m_char_idx = 0;
        private Dictionary<long, RelationInfo> m_relations = null;
        private Dictionary<long, RelationEventInfo> m_relation_events = null;//所有待处理事件

        private bool m_is_dirty = false;        //数据是否有更改
        private long m_last_save_time = 0;      //最后保存时间

        public MemberRelation()
        {
            m_relations = new Dictionary<long, RelationInfo>();
            m_relation_events = new Dictionary<long, RelationEventInfo>();
        }

        /// <summary>
        /// 从db加载数据
        /// </summary>
        public void Setup(long _char_idx)
        {
            m_is_dirty = false;
            m_char_idx = _char_idx;
            m_last_save_time = Time.timeSinceStartup;
            //查询好友
            SQLRelationHandle.QueryRelationInfo(m_char_idx, (is_load, by) =>
            {
                //查询过程如果下线
                if (m_char_idx == 0) return;
                if (is_load)
                {
                    this.Derialize(by);
                    this.SyncDataFromUnit();
                }

                //查询和自己相关的事件
                SQLRelationHandle.QueryRelationEvent(m_char_idx, HandleRelationEvent);
            });
        }
        public void Destroy()
        {
            this.Save();
            foreach (var obj in m_relations)
            {
                CommonObjectPools.Despawn(obj.Value);
            }
            m_relation_events.Clear();
            m_relations.Clear();
            m_char_idx = 0;
        }
        private float tmpLastUpdateTime = 0;
        public void Update()
        {
            if (Time.timeSinceStartup - tmpLastUpdateTime > 1000 * relation.DB_UPDATE_TIME_OFFSET)//每5分钟取一次事件
            {
                tmpLastUpdateTime = Time.timeSinceStartup;

                //查询和自己相关的事件
                SQLRelationHandle.QueryRelationEvent(m_char_idx, HandleRelationEvent);
            }
        }
        /// <summary>
        /// 同步好友最新数据：玩家上线时触发
        /// </summary>
        public void SyncDataFromUnit()
        {
            foreach(var obj in m_relations)
            {
                RelationInfo info = obj.Value;
                Unit unit = UnitManager.Instance.GetUnitByIdx(info.char_idx);
                if (unit == null)
                {//取db离线数据
                    long target_char_idx = info.char_idx;
                    PlayerInfoForGL data = CommonObjectPools.Spawn<PlayerInfoForGL>();
                    SQLCharHandle.QueryCharacterInfo(target_char_idx, data, (ret) =>
                    {
                        if(ret)
                        {
                            RelationInfo db_info;
                            if (m_relations.TryGetValue(target_char_idx, out db_info))
                                db_info.Copy(data);
                        }
                        CommonObjectPools.Despawn(data);
                        this.SyncRelation2SS(target_char_idx);
                    });
                }
                else
                {
                    info.Copy(unit.player_data);
                    this.SyncRelation2SS(info.char_idx);
                }
            }
        }
        /// <summary>
        /// 保存到db
        /// </summary>
        public void Save()
        {
            //Log.Debug("保存玩家关系:" + m_char_idx);

            ByteArray by = DBUtils.AllocDBArray();
            this.Serialize(by);
            SQLRelationHandle.UpdateCharRelation(m_char_idx, by);

            m_is_dirty = false;
            m_last_save_time = Time.timeSinceStartup;
        }
        /// <summary>
        /// 是否需要存盘
        /// </summary>
        /// <returns></returns>
        public bool NeedSave()
        {
            if (m_is_dirty && Time.timeSinceStartup - m_last_save_time > 1000 * 60 * 5)//5分钟保存一次
                return true;
            return false;
        }
        /// <summary>
        /// 广播关系事件
        /// </summary>
        /// <param name="char_idx"></param>
        /// <param name="evt"></param>
        private void BroadcastEvent(long char_idx, eRelationEvent evt)
        {
            Unit player = UnitManager.Instance.GetUnitByIdx(char_idx);
            if (player == null || !player.is_online)
                return;
            MemberRelation relation = RelationManager.Instance.GetMember(char_idx);
            if (relation == null)
                return;
            relation.OnBroadcastEvent(evt);
        }
        public void OnBroadcastEvent(eRelationEvent evt)
        {
            //查询和自己相关的事件
            SQLRelationHandle.QueryRelationEvent(m_char_idx, HandleRelationEvent);
        }

        public long char_idx
        {
            get { return m_char_idx; }
        }
        #region 序列化
        public void Serialize(ByteArray by)
        {
            foreach (var obj in m_relations)
            {
                obj.Value.Write(by);
            }
        }
	    public void Derialize(ByteArray by)
        {
            while (by.Available > 0 && by.Available >= RelationInfo.BaseSize)
            {
                RelationInfo info = CommonObjectPools.Spawn<RelationInfo>();
                info.Read(by);
                m_relations.Add(info.char_idx, info);
            }
        }
        #endregion

        #region 集合管理
        public void AddRelation(RelationInfo info)
        {
            if (m_relations.ContainsKey(info.char_idx))
                return;
            m_relations.Add(info.char_idx, info);
            m_is_dirty = true;
        }
        public void RemoveRelation(long char_idx)
        {
            RelationInfo info;
            if (!m_relations.TryGetValue(char_idx, out info))
                return;
            m_relations.Remove(char_idx);
            CommonObjectPools.Despawn(info);
            m_is_dirty = true;
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
            foreach(var obj in m_relations)
            {
                if (obj.Value.flags == flag) count++;
            }
            switch(flag)
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
        #endregion

        #region 关系操作
        /// <summary>
        /// 添加关系
        /// </summary>
        public void AddRelationCommand(RelationAddTarget target_id, eRelationFlag flag, string message)
        {
            //超过上限
            if (IsRelationFull(flag))
                return;

            //不在线了
            Unit player = UnitManager.Instance.GetUnitByIdx(m_char_idx);
            if (player == null)
                return;

            //判断是否已经存在关系，另外防止添加自己为好友
            if (target_id.type == eRelationAddType.Idx)
            {
                if (target_id.char_idx == m_char_idx) return;
                RelationInfo relation_info;
                if (m_relations.TryGetValue(target_id.char_idx, out relation_info) && relation_info.flags == flag)
                {
                    return;
                }
            }
            else
            {
                if (target_id.char_name == player.char_name) return;
                foreach (var relation_info in m_relations)
                {
                    if (relation_info.Value.char_name == target_id.char_name && relation_info.Value.flags == flag)
                    {
                        return;
                    }
                }
            }

            //如果是根据名称加好友，必须在缓存里面能查找到玩家数据
            Unit target_player = null;
            if (target_id.type == eRelationAddType.Idx)
            {
                target_player = UnitManager.Instance.GetUnitByIdx(target_id.char_idx);
            }
            else if (target_id.type == eRelationAddType.Name)
            {
                target_player = UnitManager.Instance.GetUnitByName(target_id.char_name);
                if (target_player == null)
                    return;//TODO:后期如果有需求，可以查表
                else
                    target_id.char_idx = target_player.char_idx;
            }

            //拉黑直接处理，如果有好友关系，需要先去掉好友
            if(flag == eRelationFlag.Block)
            {
                RelationInfo relation_info;
                if (m_relations.TryGetValue(target_id.char_idx, out relation_info))
                {
                    relation_info.flags = eRelationFlag.Block;
                    this.SyncRelation2SS(target_id.char_idx);
                }
                else
                {
                    if(target_player == null)
                    {
                        PlayerInfoForGL data = CommonObjectPools.Spawn<PlayerInfoForGL>();
                        SQLCharHandle.QueryCharacterInfo(target_id.char_idx, data, (ret) =>
                        {
                            if (ret && m_char_idx > 0)
                            {
                                relation_info = CommonObjectPools.Spawn<RelationInfo>();
                                relation_info.Copy(data);
                                relation_info.flags = flag;
                                this.AddRelation(relation_info);
                                this.SyncRelation2SS(target_id.char_idx);
                            }
                            CommonObjectPools.Despawn(data);
                        });
                    }
                    else
                    {
                        relation_info = CommonObjectPools.Spawn<RelationInfo>();
                        relation_info.Copy(target_player.player_data);
                        relation_info.flags = flag;
                        this.AddRelation(relation_info);
                        this.SyncRelation2SS(target_id.char_idx);
                    }
                }
            }
            
            //添加到db,需要先判断数据库是否已经有写入过，防止重复写入
            //注：如果赠送可以同时存在的话，这里需要屏蔽赠送类型
            SQLRelationHandle.QueryExistsRelationEvent(m_char_idx, target_id.char_idx, flag, (event_idx) =>
            {
                if (event_idx == 0 && m_char_idx > 0)
                {//保存事件
                    RelationEventInfo e_info = new RelationEventInfo();
                    e_info.target_char_idx = target_id.char_idx;
                    e_info.source_char_idx = m_char_idx;
                    e_info.event_type = eRelationEvent.Add;
                    e_info.bin_content.bin_add_content.char_name = player.char_name;
                    e_info.bin_content.bin_add_content.message = message;
                    e_info.bin_content.bin_add_content.flag = flag;
                    SQLRelationHandle.InsertRelationEvent(e_info);

                    //立刻通知接受者
                    this.BroadcastEvent(e_info.target_char_idx, e_info.event_type);
                }
            });
        }
        /// <summary>
        /// 移除关系
        /// </summary>
        public void RemoveRelationCommand(long target_char_idx)
        {
            //不在线了
            Unit player = UnitManager.Instance.GetUnitByIdx(m_char_idx);
            if (player == null)
                return;

            //是否已经存在关系
            RelationInfo relation_info = null;
            if (m_relations.TryGetValue(target_char_idx, out relation_info))
            {
                this.RemoveRelation(target_char_idx);

                //通知逻辑服，由逻辑服转发给客户端
                gl2ss.RelationRemove msg_remove = PacketPools.Get(gl2ss.msg.RELATION_REMOVE) as gl2ss.RelationRemove;
                msg_remove.char_idx = m_char_idx;
                msg_remove.target_idx = target_char_idx;
                ForServerNetManager.Instance.Send(player.ss_srv_uid, msg_remove);

                //添加到db,需要先判断数据库是否已经有写入过，防止重复写入
                SQLRelationHandle.QueryExistsRelationEvent(m_char_idx, target_char_idx, relation_info.flags, (event_idx) =>
                {
                    if (event_idx == 0 && m_char_idx > 0)
                    {//保存事件
                        RelationEventInfo e_info = new RelationEventInfo();
                        e_info.target_char_idx = target_char_idx;
                        e_info.source_char_idx = m_char_idx;
                        e_info.event_type = eRelationEvent.Delete;
                        SQLRelationHandle.InsertRelationEvent(e_info);
                        //立刻通知接受者
                        this.BroadcastEvent(e_info.target_char_idx, e_info.event_type);
                    }
                });
            }
        }
        /// <summary>
        /// 申请反馈
        /// </summary>
        public void ApplyRelationCommand(long event_idx, long target_char_idx, eRelationApplyCmd cmd)
        {
            //是否存在申请事件
            RelationEventInfo relation_evt;
            if (!m_relation_events.TryGetValue(event_idx, out relation_evt))
            {
                return;
            }

            switch(relation_evt.event_type)
            {
                case eRelationEvent.Add:
                    {
                        eRelationFlag flag = relation_evt.bin_content.bin_add_content.flag;
                        if (cmd == eRelationApplyCmd.Agree)
                        {
                            //超过上限
                            if (IsRelationFull(flag))
                            {
                                SQLRelationHandle.RemoveRelationEvent(event_idx);
                                m_relation_events.Remove(event_idx);
                                return;
                            }

                            //是否已经存在相同关系：是则返回；非相同关系先移除，再添加
                            RelationInfo relation_info;
                            if (m_relations.TryGetValue(target_char_idx, out relation_info))
                            {
                                if (relation_info.flags == flag)
                                {
                                    SQLRelationHandle.RemoveRelationEvent(event_idx);
                                    m_relation_events.Remove(event_idx);
                                    return;
                                }
                                else
                                {
                                    this.RemoveRelation(target_char_idx);
                                }
                            }

                            //如果对方在线，则取对方身上的数据；否则取数据库的数据
                            Unit target_player = UnitManager.Instance.GetUnitByIdx(target_char_idx);
                            if (target_player != null)
                            {
                                relation_info = CommonObjectPools.Spawn<RelationInfo>();
                                relation_info.Copy(target_player.player_data);
                                relation_info.flags = flag;
                                this.AddRelation(relation_info);
                                this.SyncRelation2SS(target_char_idx);
                            }
                            else
                            {
                                PlayerInfoForGL data = CommonObjectPools.Spawn<PlayerInfoForGL>();
                                SQLCharHandle.QueryCharacterInfo(target_char_idx, data, (ret) =>
                                {
                                    if (ret && m_char_idx > 0)
                                    {
                                        relation_info = CommonObjectPools.Spawn<RelationInfo>();
                                        relation_info.Copy(data);
                                        relation_info.flags = flag;
                                        this.AddRelation( relation_info);
                                        this.SyncRelation2SS(target_char_idx);
                                    }
                                    CommonObjectPools.Despawn(data);
                                });
                            }
                        }
                        else
                        {
                            //undo
                        }
                        //写入事件
                        RelationEventInfo e_info = new RelationEventInfo();
                        e_info.target_char_idx = target_char_idx;
                        e_info.source_char_idx = m_char_idx;
                        e_info.event_type = eRelationEvent.Agree;
                        e_info.bin_content.bin_agree_content.flag = flag;
                        e_info.bin_content.bin_agree_content.cmd = cmd;
                        SQLRelationHandle.InsertRelationEvent(e_info);
                        //立刻通知接受者
                        this.BroadcastEvent(e_info.target_char_idx, e_info.event_type);
                    }
                    break;
            }

            //清除处理过的事件
            SQLRelationHandle.RemoveRelationEvent(event_idx);

            //从事件列表移除
            m_relation_events.Remove(event_idx);
        }
        /// <summary>
        /// 好友赠送
        /// </summary>
        public void FriendGiveCommand(long target_char_idx, ItemID item_id)
        {
            Unit player = UnitManager.Instance.GetUnitByIdx(m_char_idx);
            if (player == null)
                return;

            //先查找是否有好友
            RelationInfo relation_info;
            if (!m_relations.TryGetValue(target_char_idx, out relation_info) || relation_info.flags != eRelationFlag.Friend)
                return;

            //赠送是否合法
            if (!item.IsValidItem(item_id))
                return;

            //写入事件
            RelationEventInfo e_info = new RelationEventInfo();
            e_info.target_char_idx = target_char_idx;
            e_info.source_char_idx = m_char_idx;
            e_info.event_type = eRelationEvent.Give;
            e_info.bin_content.bin_give_content.char_name = player.char_name;
            e_info.bin_content.bin_give_content.item_id = item_id;
            SQLRelationHandle.InsertRelationEvent(e_info);
            //立刻通知接受者：这里先注释，不需要及时通知
            //this.BroadcastEvent(e_info.target_char_idx, e_info.event_type);
        }
        /// <summary>
        /// 处理关系事件
        /// </summary>
        private void HandleRelationEvent(List<RelationEventInfo> list_event)
        {
            //查询过程中如果下线
            if (m_char_idx == 0) return;

            Unit player = UnitManager.Instance.GetUnitByIdx(m_char_idx);
            if (player == null)
                return;

            foreach(var relation_evt in list_event)
            {    
                switch(relation_evt.event_type)
                {
                    case eRelationEvent.Add://对方申请加你好友
                        {
                            //超过上限
                            if (IsRelationFull(relation_evt.bin_content.bin_add_content.flag))
                            {
                                //清除处理过的事件
                                SQLRelationHandle.RemoveRelationEvent(relation_evt.event_idx);
                                continue;
                            }

                            //是否已经存在相同关系
                            RelationInfo relation_info = null;
                            if (m_relations.TryGetValue(relation_evt.source_char_idx, out relation_info) && relation_info.flags == relation_evt.bin_content.bin_add_content.flag)
                            {
                                //清除处理过的事件
                                SQLRelationHandle.RemoveRelationEvent(relation_evt.event_idx);
                                continue;
                            }

                            //拉黑不需要同意，所以不添加事件
                            if (relation_evt.bin_content.bin_add_content.flag == eRelationFlag.Block)
                            {
                                if (m_relations.TryGetValue(relation_evt.source_char_idx, out relation_info))
                                {//已经存在其他关系，则用新关系覆盖旧关系
                                    relation_info.flags = eRelationFlag.Block;
                                    this.SyncRelation2SS(relation_evt.source_char_idx);
                                }
                                else
                                {
                                    Unit target_player = UnitManager.Instance.GetUnitByIdx(relation_evt.source_char_idx);
                                    if (target_player == null)
                                    {
                                        PlayerInfoForGL data = CommonObjectPools.Spawn<PlayerInfoForGL>();
                                        SQLCharHandle.QueryCharacterInfo(relation_evt.source_char_idx, data, (ret) =>
                                        {
                                            if (ret && m_char_idx > 0)
                                            {
                                                relation_info = CommonObjectPools.Spawn<RelationInfo>();
                                                relation_info.Copy(data);
                                                relation_info.flags = eRelationFlag.Block;
                                                this.AddRelation(relation_info);
                                                this.SyncRelation2SS(relation_evt.source_char_idx);
                                            }
                                            CommonObjectPools.Despawn(data);
                                        });
                                    }
                                    else
                                    {
                                        relation_info = CommonObjectPools.Spawn<RelationInfo>();
                                        relation_info.Copy(target_player.player_data);
                                        relation_info.flags = eRelationFlag.Block;
                                        this.AddRelation(relation_info);
                                        this.SyncRelation2SS(relation_evt.source_char_idx);
                                    }
                                }

                                //清除处理过的事件
                                SQLRelationHandle.RemoveRelationEvent(relation_evt.event_idx);
                                m_relation_events.Remove(relation_evt.event_idx);
                            }
                            else
                            {
                                //添加关系事件
                                if (m_relation_events.ContainsKey(relation_evt.event_idx))
                                    continue;
                                else
                                    m_relation_events.Add(relation_evt.event_idx, relation_evt);

                                //通知逻辑服，由逻辑服转发给客户端
                                gl2ss.RelationAdd msg = PacketPools.Get(gl2ss.msg.RELATION_ADD) as gl2ss.RelationAdd;
                                msg.event_idx = relation_evt.event_idx;
                                msg.char_idx = m_char_idx;
                                msg.player_id.Set(relation_evt.source_char_idx, relation_evt.bin_content.bin_add_content.char_name);
                                msg.message = relation_evt.bin_content.bin_add_content.message;
                                msg.flag = relation_evt.bin_content.bin_add_content.flag;
                                ForServerNetManager.Instance.Send(player.ss_srv_uid, msg);
                            }
                            break;
                        }

                    case eRelationEvent.Delete://对方删除了你
                        {
                            if (m_relations.ContainsKey(relation_evt.source_char_idx))
                            {
                                this.RemoveRelation(relation_evt.source_char_idx);

                                //通知逻辑服，由逻辑服转发给客户端
                                gl2ss.RelationRemove msg_remove = PacketPools.Get(gl2ss.msg.RELATION_REMOVE) as gl2ss.RelationRemove;
                                msg_remove.char_idx = m_char_idx;
                                msg_remove.target_idx = relation_evt.source_char_idx;
                                ForServerNetManager.Instance.Send(player.ss_srv_uid, msg_remove);
                            }

                            //清除处理过的事件
                            SQLRelationHandle.RemoveRelationEvent(relation_evt.event_idx);
                            m_relation_events.Remove(relation_evt.event_idx);
                            break;
                        }

                    case eRelationEvent.Agree://对方同意了你的申请
                        {
                            if(relation_evt.bin_content.bin_agree_content.cmd == eRelationApplyCmd.Agree)
                            {
                                //如果对方在线，则取对方身上的数据；否则取数据库的数据
                                Unit target_player = UnitManager.Instance.GetUnitByIdx(relation_evt.source_char_idx);
                                if (target_player != null)
                                {
                                    this.RemoveRelation(relation_evt.source_char_idx);

                                    RelationInfo relation_info = CommonObjectPools.Spawn<RelationInfo>();
                                    relation_info.Copy(target_player.player_data);
                                    relation_info.flags = relation_evt.bin_content.bin_agree_content.flag;
                                    this.AddRelation(relation_info);
                                    this.SyncRelation2SS(relation_evt.source_char_idx);

                                    //清除处理过的事件
                                    SQLRelationHandle.RemoveRelationEvent(relation_evt.event_idx);
                                    m_relation_events.Remove(relation_evt.event_idx);
                                }
                                else
                                {
                                    PlayerInfoForGL data = CommonObjectPools.Spawn<PlayerInfoForGL>();
                                    SQLCharHandle.QueryCharacterInfo(relation_evt.source_char_idx, data, (ret) =>
                                    {
                                        if (ret && m_char_idx > 0)
                                        {
                                            this.RemoveRelation(relation_evt.source_char_idx);

                                            RelationInfo relation_info = CommonObjectPools.Spawn<RelationInfo>();
                                            relation_info.Copy(data);
                                            relation_info.flags = relation_evt.bin_content.bin_agree_content.flag;
                                            this.AddRelation(relation_info);
                                            this.SyncRelation2SS(relation_evt.source_char_idx);
                                        }
                                        CommonObjectPools.Despawn(data);

                                        //清除处理过的事件
                                        SQLRelationHandle.RemoveRelationEvent(relation_evt.event_idx);
                                        m_relation_events.Remove(relation_evt.event_idx);
                                    });
                                }
                            }
                            else
                            {
                                //清除处理过的事件
                                SQLRelationHandle.RemoveRelationEvent(relation_evt.event_idx);
                                m_relation_events.Remove(relation_evt.event_idx);
                            }

                            break;
                        }

                    case eRelationEvent.Give:
                        {
                            Log.Info("赠送 src:" + relation_evt.source_char_idx + " dst:" + relation_evt.target_char_idx);
                            RelationEventContent.GiveContent give_info = relation_evt.bin_content.bin_give_content;
                            //发给ss处理
                            if(item.IsValidItem(give_info.item_id))
                            {
                                gl2ss.RelationGive msg_give = PacketPools.Get(gl2ss.msg.RELATION_GIVE) as gl2ss.RelationGive;
                                msg_give.char_idx = m_char_idx;
                                msg_give.src_player_id.Set(relation_evt.source_char_idx, give_info.char_name);
                                msg_give.item_id = give_info.item_id;
                                ForServerNetManager.Instance.Send(player.ss_srv_uid, msg_give);
                            }

                            //清除处理过的事件
                            SQLRelationHandle.RemoveRelationEvent(relation_evt.event_idx);
                            m_relation_events.Remove(relation_evt.event_idx);
                            break;
                        }
                }
            }
        }
        /// <summary>
        /// 下发关系列表
        /// </summary>
        private void SyncRelation2SS(long target_char_idx)
        {
            Unit player = UnitManager.Instance.GetUnitByIdx(m_char_idx);
            if (player == null)
                return;

            RelationInfo relation_info = null;
            if (m_relations.TryGetValue(target_char_idx, out relation_info))
            {
                gl2ss.RelationList msg = PacketPools.Get(gl2ss.msg.RELATION_LIST) as gl2ss.RelationList;
                msg.char_idx = m_char_idx;
                msg.relation_info.Copy(relation_info);
                ForServerNetManager.Instance.Send(player.ss_srv_uid, msg);
            }
        }
        #endregion

        #region 属性改变，同步信息
        /// <summary>
        /// 属性改变
        /// </summary>
        public void UpdateAttribute(long target_char_idx, eUnitModType type, long value)
        {
            RelationInfo info = GetRelation(target_char_idx);
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
            m_is_dirty = true;
        }
        public void UpdateAttribute(long target_char_idx, eUnitModType type, string value)
        {
            RelationInfo info = GetRelation(target_char_idx);
            if (info == null) return;

            switch (type)
            {
                case eUnitModType.UMT_char_name: info.char_name = value; break;
                default: return;
            }
            m_is_dirty = true;
        }
        #endregion
    }
}
