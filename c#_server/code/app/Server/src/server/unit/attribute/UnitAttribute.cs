using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 单位属性
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public class UnitAttribute
    {
        protected Unit m_owner_unit = null;

        public UnitAttribute(Unit unit)
        {
            m_owner_unit = unit;
        }

        /// <summary>
        /// 根据Key值获取角色的类型为int的属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual long GetAttribInteger(eUnitModType type) { return 0; }
        /// <summary>
        /// 根据Key值设置角色的类型为int的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        /// <param name="sync_db"></param>
        /// <param name="sync_client"></param>
        /// <param name="sync_ws"></param>
        /// <param name="sync_gl"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual bool SetAttribInteger(eUnitModType type, long nValue, bool sync_db = true, bool sync_client = true, bool sync_ws = true, bool sync_gl = true, bool sync_fs = true, eUnitAttrAction action = eUnitAttrAction.Unknow) { return false; }
        /// <summary>
        ///  根据Key值改变角色的类型为int的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        /// <param name="operate"></param>
        /// <param name="sync_db"></param>
        /// <param name="sync_client"></param>
        /// <param name="sync_ws"></param>
        /// <param name="sync_gl"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual bool ChangeAttribInteger(eUnitModType type, long nValue, eUnitAttrOperate operate, bool sync_db = true, bool sync_client = true, bool sync_ws = true, bool sync_gl = true, bool sync_fs = true, eUnitAttrAction action = eUnitAttrAction.Unknow) { return false; }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual string GetAttribString(eUnitModType type) { return ""; }
        public virtual bool SetAttribString(eUnitModType type, string nValue, bool sync_db = true, bool sync_client = true, bool sync_ws = true, bool sync_gl = true, bool sync_fs = true, eUnitAttrAction action = eUnitAttrAction.Unknow) { return false; }
    
        /// <summary>
        /// 是否全屏广播属性：全屏广播属性会广播给周围其他玩家，谨慎使用
        /// </summary>
        /// <returns></returns>
        public virtual eUnitAttrObserver GetObserverType(eUnitModType type) { return eUnitAttrObserver.Single; }
    }
    /// <summary>
    /// 玩家属性
    /// </summary>
    public class PlayerAttribute : UnitAttribute
    {
        private PlayerInfoForSS m_player_info = new PlayerInfoForSS();

        public PlayerAttribute(Unit unit)
            : base(unit)
        {
        }
        public PlayerInfoForSS player_info
        {
            get { return m_player_info;  }
        }
        public void SetAccountIdx(long account_index)
        {
            m_player_info.account_index = account_index;
        }
        public void SetCharIdx(long char_idx)
        {
            m_player_info.char_idx = char_idx;
        }
        public override long GetAttribInteger(eUnitModType type)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return 0;

            switch(type)
            {
                case eUnitModType.UMT_char_type: return m_player_info.char_type;
                case eUnitModType.UMT_fs_uid: return m_player_info.fs_id;
                case eUnitModType.UMT_flags: return m_player_info.flags;
                case eUnitModType.UMT_model_idx: return m_player_info.model_idx;
                case eUnitModType.UMT_job: return m_player_info.job;
                case eUnitModType.UMT_level: return m_player_info.level;
                case eUnitModType.UMT_exp: return m_player_info.exp;
                case eUnitModType.UMT_energy: return m_player_info.energy;
                case eUnitModType.UMT_gold: return m_player_info.gold;
                case eUnitModType.UMT_coin: return m_player_info.coin;
                case eUnitModType.UMT_hp: return m_player_info.hp;
                case eUnitModType.UMT_vip_grade: return m_player_info.vip_grade;
                case eUnitModType.UMT_vip_flags: return m_player_info.vip_flags;

                case eUnitModType.UMT_base_energy: return m_player_info.energy;
                case eUnitModType.UMT_base_hurt: return m_player_info.hurt;
                case eUnitModType.UMT_base_run_speed: return m_player_info.run_speed;

                case eUnitModType.UMT_hp_max: return m_player_info.hp_max;
                case eUnitModType.UMT_hurt: return m_player_info.hurt;
                case eUnitModType.UMT_range: return m_player_info.range;
                case eUnitModType.UMT_run_speed: return m_player_info.run_speed;
                default: return 0;
            }
        }

        public override bool SetAttribInteger(eUnitModType type, long nValue, bool sync_db = true, bool sync_client = true, bool sync_ws = true, bool sync_gl = true, bool sync_fs = true, eUnitAttrAction action = eUnitAttrAction.Unknow)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return false;

            switch (type)
            {
                case eUnitModType.UMT_char_type: m_player_info.char_type = (byte)nValue; break;
                case eUnitModType.UMT_fs_uid: m_player_info.fs_id = (ushort)nValue; break;
                case eUnitModType.UMT_flags: m_player_info.flags = (uint)nValue; break;
                case eUnitModType.UMT_scene_type: m_player_info.scene_type_idx = (uint)nValue; break;
                case eUnitModType.UMT_model_idx: m_player_info.model_idx = (uint)nValue; break;
                case eUnitModType.UMT_job: m_player_info.job = (byte)nValue; break;
                case eUnitModType.UMT_level: m_player_info.level = (ushort)nValue; break;
                case eUnitModType.UMT_exp: m_player_info.exp = (uint)nValue; break;
                case eUnitModType.UMT_energy: m_player_info.energy = (uint)nValue; break;
                case eUnitModType.UMT_gold: m_player_info.gold = (uint)nValue; break;
                case eUnitModType.UMT_coin: m_player_info.coin = (uint)nValue; break;
                case eUnitModType.UMT_hp: m_player_info.hp = (uint)nValue; break;
                case eUnitModType.UMT_vip_grade: m_player_info.vip_grade = (uint)nValue; break;
                case eUnitModType.UMT_vip_flags: m_player_info.vip_flags = (uint)nValue; break;

                case eUnitModType.UMT_base_energy: m_player_info.energy = (uint)nValue; break;
                case eUnitModType.UMT_base_hurt: m_player_info.hurt = (uint)nValue; break;
                case eUnitModType.UMT_base_run_speed: m_player_info.run_speed = (uint)nValue; break;

                case eUnitModType.UMT_hp_max: m_player_info.hp_max = (uint)nValue; break;
                case eUnitModType.UMT_hurt: m_player_info.hurt = (uint)nValue; break;
                case eUnitModType.UMT_range: m_player_info.range = (uint)nValue; break;
                case eUnitModType.UMT_run_speed: m_player_info.run_speed = (uint)nValue; break;

                case eUnitModType.UMT_time_last_login: m_player_info.time_last_login = (long)nValue; break;
                case eUnitModType.UMT_time_last_logout: m_player_info.time_last_logout = (long)nValue; break;
                default: return false;
            }
            if (sync_client && IsNeedSyncClient(type)) Send2Client(type, nValue, action);
            if (sync_db) Send2DB(type, nValue);
            if (sync_ws && IsNeedSyncWS(type)) Send2WS(type, nValue);
            if (sync_gl && IsNeedSyncGL(type)) Send2GL(type, nValue);
            if (sync_fs && IsNeedSyncFS(type)) Send2FS(type, nValue);
            return true; 
        }

        public override bool ChangeAttribInteger(eUnitModType type, long nValue, eUnitAttrOperate operate, bool sync_db = true, bool sync_client = true, bool sync_ws = true, bool sync_gl = true, bool sync_fs = true, eUnitAttrAction action = eUnitAttrAction.Unknow)
        {
            if (nValue == 0) return false;
            Player player = m_owner_unit as Player;
            if (player == null) return false;

            long base_value = this.GetAttribInteger(type);
            long dst_value = base_value;
            switch(operate)
            {
                case eUnitAttrOperate.Add:
                    dst_value += nValue;
                    break;
                case eUnitAttrOperate.Sub:
                    dst_value -= nValue;
                    break;
                case eUnitAttrOperate.Mul:
                    dst_value *= nValue;
                    break;
                case eUnitAttrOperate.Div:
                    dst_value /= nValue;
                    break;
            }
            if (dst_value == base_value) return false;
            if (sync_client && IsNeedSyncClient(type)) Send2Client(type, dst_value, action);
            if (sync_db) Send2DB(type, nValue);
            if (sync_ws && IsNeedSyncWS(type)) Send2WS(type, nValue);
            if (sync_gl && IsNeedSyncGL(type)) Send2GL(type, nValue);
            if (sync_fs && IsNeedSyncFS(type)) Send2FS(type, nValue);

            return true; 
        }

        public override string GetAttribString(eUnitModType type) 
        {
            Player player = m_owner_unit as Player;
            if (player == null) return "";

            switch (type)
            {
                case eUnitModType.UMT_char_name: return m_player_info.char_name;
                default: return "";
            }
        }
        public override bool SetAttribString(eUnitModType type, string nValue, bool sync_db = true, bool sync_client = true, bool sync_ws = true, bool sync_gl = true, bool sync_fs = true, eUnitAttrAction action = eUnitAttrAction.Unknow) 
        {
            Player player = m_owner_unit as Player;
            if (player == null) return false;

            switch (type)
            {
                case eUnitModType.UMT_char_name: m_player_info.char_name = nValue; break;
                default: return false;
            }

            if (sync_client && IsNeedSyncClient(type)) Send2Client(type, nValue, action);
            if (sync_db) Send2DB(type, nValue);
            if (sync_ws && IsNeedSyncWS(type)) Send2WS(type, nValue);
            if (sync_gl && IsNeedSyncGL(type)) Send2GL(type, nValue);
            if (sync_fs && IsNeedSyncFS(type)) Send2FS(type, nValue);
            return true; 
        }
        public bool SetAttrib(eUnitModType type, long nValue, string strValue, bool sync_db = true, bool sync_client = true, bool sync_ws = true, bool sync_gl = true, bool sync_fs = true, eUnitAttrAction action = eUnitAttrAction.Unknow)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return false;

            switch (type)
            {
                case eUnitModType.UMT_char_name:
                    return SetAttribString(type, strValue, sync_db, sync_client, sync_ws, sync_gl, sync_fs, action);
                default:
                    return SetAttribInteger(type, nValue, sync_db, sync_client, sync_ws, sync_gl, sync_fs, action);
            }
        }
        public override eUnitAttrObserver GetObserverType(eUnitModType type)
        {
            eUnitAttrObserver observer = eUnitAttrObserver.Single;
            switch (type)
            {
                case eUnitModType.UMT_char_type:
                case eUnitModType.UMT_model_idx:
                case eUnitModType.UMT_level:
                case eUnitModType.UMT_gold:
                case eUnitModType.UMT_coin:
                case eUnitModType.UMT_vip_grade:
                    observer = eUnitAttrObserver.BoardcaseScreen;
                    break;
            }
            return observer;
        }
        /// <summary>
        /// 属性是否需要同步到客户端
        /// </summary>
        private bool IsNeedSyncClient(eUnitModType type)
        {
            switch (type)
            {
                case eUnitModType.UMT_char_type:
                case eUnitModType.UMT_char_name:
                case eUnitModType.UMT_flags:
                case eUnitModType.UMT_model_idx:
                case eUnitModType.UMT_job:
                case eUnitModType.UMT_level:
                case eUnitModType.UMT_exp:
                case eUnitModType.UMT_gold:
                case eUnitModType.UMT_coin:
                case eUnitModType.UMT_hp:
                case eUnitModType.UMT_vip_grade:
                case eUnitModType.UMT_vip_flags:
                case eUnitModType.UMT_base_hurt:
                case eUnitModType.UMT_base_run_speed:
                case eUnitModType.UMT_base_energy:
                case eUnitModType.UMT_hp_max:
                case eUnitModType.UMT_energy_max:
                case eUnitModType.UMT_hurt:
                case eUnitModType.UMT_range:
                case eUnitModType.UMT_run_speed:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 属性是否需要同步到fs：每次战斗，都会发一份最新数据给战斗服，所以一般数据不需要同步给fs，除非是战斗中可以改变的数据
        /// </summary>
        private bool IsNeedSyncFS(eUnitModType type)
        {
            switch (type)
            {
                default:
                    return false;
            }
        }
        /// <summary>
        /// 属性是否需要同步到ws
        /// </summary>
        private bool IsNeedSyncWS(eUnitModType type)
        {
            switch (type)
            {
                case eUnitModType.UMT_char_type:
                case eUnitModType.UMT_char_name:
                case eUnitModType.UMT_flags:
                case eUnitModType.UMT_model_idx:
                case eUnitModType.UMT_job:
                case eUnitModType.UMT_level:
                case eUnitModType.UMT_exp:
                case eUnitModType.UMT_gold:
                case eUnitModType.UMT_coin:
                case eUnitModType.UMT_vip_grade:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 属性是否需要同步到gl
        /// </summary>
        private bool IsNeedSyncGL(eUnitModType type)
        {
            switch (type)
            {
                case eUnitModType.UMT_char_type:
                case eUnitModType.UMT_char_name:
                case eUnitModType.UMT_flags:
                case eUnitModType.UMT_model_idx:
                case eUnitModType.UMT_job:
                case eUnitModType.UMT_level:
                case eUnitModType.UMT_exp:
                case eUnitModType.UMT_gold:
                case eUnitModType.UMT_coin:
                case eUnitModType.UMT_vip_grade:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 发给客户端-int
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2Client(eUnitModType type, long nValue, eUnitAttrAction action)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            ss2c.NotifyUpdatePlayerAttribInteger msg = PacketPools.Get(ss2c.msg.UNIT_MODIFY_INT) as ss2c.NotifyUpdatePlayerAttribInteger;
            msg.client_uid = player.client_uid;
            msg.unit_idx.Set(player.unit_type, player.obj_type, player.char_idx);
            msg.action = action;
            msg.type = type;
            msg.value = nValue;
            Send2Client(type, msg);
        }
        /// <summary>
        /// 发给客户端-string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2Client(eUnitModType type, string nValue, eUnitAttrAction action)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            ss2c.NotifyUpdatePlayerAttribString msg = PacketPools.Get(ss2c.msg.UNIT_MODIFY_STRING) as ss2c.NotifyUpdatePlayerAttribString;
            msg.client_uid = player.client_uid;
            msg.unit_idx.Set(player.unit_type, player.obj_type, player.char_idx);
            msg.action = action;
            msg.type = type;
            msg.value = nValue;
            Send2Client(type, msg);
        }
        private void Send2Client(eUnitModType type, PackBaseS2C msg)
        {
            eUnitAttrObserver oberver = this.GetObserverType(type);
            switch (oberver)
            {
                case eUnitAttrObserver.Single:
                    ServerNetManager.Instance.SendProxy(msg.client_uid, msg, false);
                    break;
                case eUnitAttrObserver.BoardcaseScreen:
                    {
                        ServerNetManager.Instance.SendProxy(msg.client_uid, msg, false);
                        //转发其他同屏玩家
                        List<long> list_unit = AOIManager.Instance.GetScreenUnit(m_owner_unit.obj_idx);
                        if (list_unit != null && list_unit.Count > 0)
                        {
                            foreach (var idx in list_unit)
                            {
                                Player player = UnitManager.Instance.GetUnitByIdx(idx) as Player;
                                if (player == null) continue;
                                ServerNetManager.Instance.SendProxy(player.client_uid, msg, false);
                            }
                        }
                    }
                    break;
                case eUnitAttrObserver.Boardcase:
                    ServerNetManager.Instance.SendProxy(msg.client_uid, msg, true);
                    break;
            }
        }
        /// <summary>
        /// 发给db
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2DB(eUnitModType type, long nValue)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            player.SetDirty();
        }
        /// <summary>
        /// 发给db-string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2DB(eUnitModType type, string nValue)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            player.SetDirty();
        }
        /// <summary>
        /// 发给gl
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2GL(eUnitModType type, long nValue)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            ss2gl.NotifyUpdatePlayerAttribInteger msg = PacketPools.Get(ss2gl.msg.UNIT_MODIFY_INT) as ss2gl.NotifyUpdatePlayerAttribInteger;
            msg.char_idx = player.char_idx;
            msg.type = type;
            msg.value = nValue;
            ServerNetManager.Instance.Send2GL(msg);
        }
        /// <summary>
        /// 发给gl-string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2GL(eUnitModType type, string nValue)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            ss2gl.NotifyUpdatePlayerAttribString msg = PacketPools.Get(ss2gl.msg.UNIT_MODIFY_STRING) as ss2gl.NotifyUpdatePlayerAttribString;
            msg.char_idx = player.char_idx;
            msg.type = type;
            msg.value = nValue;
            ServerNetManager.Instance.Send2GL(msg);
        }
        /// <summary>
        /// 发给ws
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2WS(eUnitModType type, long nValue)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            ss2ws.NotifyUpdatePlayerAttribInteger msg = PacketPools.Get(ss2ws.msg.UNIT_MODIFY_INT) as ss2ws.NotifyUpdatePlayerAttribInteger;
            msg.char_idx = player.char_idx;
            msg.type = type;
            msg.value = nValue;
            ServerNetManager.Instance.Send2WS(msg);
        }
        /// <summary>
        /// 发给ws-string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2WS(eUnitModType type, string nValue)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            ss2ws.NotifyUpdatePlayerAttribString msg = PacketPools.Get(ss2ws.msg.UNIT_MODIFY_STRING) as ss2ws.NotifyUpdatePlayerAttribString;
            msg.char_idx = player.char_idx;
            msg.type = type;
            msg.value = nValue;
            ServerNetManager.Instance.Send2WS(msg);
        }
        /// <summary>
        /// 发给fs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2FS(eUnitModType type, long nValue)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            ss2fs.NotifyUpdatePlayerAttribInteger msg = PacketPools.Get(ss2fs.msg.UNIT_MODIFY_INT) as ss2fs.NotifyUpdatePlayerAttribInteger;
            msg.char_idx = player.char_idx;
            msg.type = type;
            msg.value = nValue;
            ServerNetManager.Instance.Send2FS(player.fs_uid, msg);
        }
        /// <summary>
        /// 发给fs-string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nValue"></param>
        private void Send2FS(eUnitModType type, string nValue)
        {
            Player player = m_owner_unit as Player;
            if (player == null) return;

            ss2fs.NotifyUpdatePlayerAttribString msg = PacketPools.Get(ss2fs.msg.UNIT_MODIFY_STRING) as ss2fs.NotifyUpdatePlayerAttribString;
            msg.char_idx = player.char_idx;
            msg.type = type;
            msg.value = nValue;
            ServerNetManager.Instance.Send2FS(player.fs_uid, msg);
        }
    }
}
