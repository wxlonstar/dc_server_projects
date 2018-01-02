using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 管理服务器单位
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public class UnitManager : Singleton<UnitManager>
    {
        private Dictionary<long, Unit> m_units;//所有单位集合
        private Dictionary<string, long> m_name_players;//姓名->角色
        private Dictionary<long, long> m_account_players;//账号->角色
        private Dictionary<ClientUID, long> m_client_players;//client_uid->角色
        private List<long> m_players;//所有玩家列表

        public UnitManager()
        {
            m_units = new Dictionary<long,Unit>();
            m_name_players = new Dictionary<string, long>();
            m_account_players = new Dictionary<long, long>();
            m_client_players = new Dictionary<ClientUID, long>();
            m_players = new List<long>();
        }

        public void Setup()
        {

        }
        public void Destroy()
        {
            foreach (var obj in m_units)
            {
                obj.Value.OnLeave();
            }
            m_units.Clear();
            m_client_players.Clear();
        }
        public void Tick()
        {
            foreach (var obj in m_units)
            {
                obj.Value.Update();
            }
            this.TickSave();
            this.TickUploadCount();
        }
        /// <summary>
        /// 存盘
        /// </summary>
        private void TickSave()
        {
            int update_count = 0;
            Player player = null;
            foreach (var obj in m_units)
            {
                player = obj.Value as Player;
                if (player != null && player.NeedSave())
                {
                    player.Save();
                    if (++update_count > 60) break;//当次循环最大保存数量
                }
            }
        }
        /// <summary>
        /// 上报玩家数量
        /// </summary>
        private long tmpLastUploadTime = 0;
        private void TickUploadCount()
        {
            if(Time.timeSinceStartup - tmpLastUploadTime >= GlobalID.UPLOAD_ONLINE_COUNT_TIME * 1000)
            {
                ss2ws.OnlineCount msg = PacketPools.Get(ss2ws.msg.ONLINE_COUNT) as ss2ws.OnlineCount;
                msg.count = (ushort)this.GetPlayerCount();
                ServerNetManager.Instance.Send2WS(msg);

                tmpLastUploadTime = Time.timeSinceStartup;
            }
        }
        #region 单位管理
        public bool AddUnit(Unit unit)
        {
            if (unit == null) return false;
            if (m_units.ContainsKey(unit.obj_idx)) return false;

            m_units.Add(unit.obj_idx, unit);
            if(unit is Player)
            {
                Player player = unit as Player;
                m_name_players.Remove(player.char_name);
                m_client_players.Remove(player.client_uid);
                m_account_players.Remove(player.account_idx);
                m_name_players.Add(player.char_name, unit.obj_idx);
                m_client_players.Add(player.client_uid, unit.obj_idx);
                m_account_players.Add(player.account_idx, unit.obj_idx);
                if (!m_players.Contains(unit.obj_idx)) m_players.Add(unit.obj_idx);
            }
            unit.OnEnter();
            return true;
        }
        public void RemoveUnit(Unit unit)
        {
            if (unit == null) return;
            unit.OnLeave();
            if (unit is Player)
            {
                Player player = unit as Player;
                m_name_players.Remove(player.char_name);
                m_client_players.Remove(player.client_uid);
                m_account_players.Remove(player.account_idx);
                m_players.Remove(unit.obj_idx);
            }
            m_units.Remove(unit.obj_idx);
            CommonObjectPools.Despawn(unit);
        }
        public void RemoveUnit(long idx)
        {
            Unit unit = null;
            if (m_units.TryGetValue(idx, out unit))
                this.RemoveUnit(unit);
        }

        public Unit GetUnitByIdx(long idx)
        {
            Unit unit = null;
            if (m_units.TryGetValue(idx, out unit))
                return unit;
            return null;
        }
        public bool HasUnit(long idx)
        {
            return m_units.ContainsKey(idx);
        }
        #endregion

        #region 玩家
        public Player GetPlayerByIdx(long char_idx)
        {
            Unit unit = this.GetUnitByIdx(char_idx);
            if (unit != null)
                return unit as Player;
            return null;
        }
        public Player GetPlayerByName(string char_name)
        {
            long char_idx = 0;
            if (m_name_players.TryGetValue(char_name, out char_idx))
            {
                return GetUnitByIdx(char_idx) as Player;
            }
            return null;
        }
        public Player GetPlayerByAccount(long account)
        {
            long char_idx = 0;
            if(m_account_players.TryGetValue(account, out char_idx))
            {
                return GetUnitByIdx(char_idx) as Player;
            }
            return null;
        }
        public Player GetPlayerByClientUID(ClientUID uid)
        {
            long char_idx = 0;
            if (m_client_players.TryGetValue(uid, out char_idx))
            {
                return GetUnitByIdx(char_idx) as Player;
            }
            return null;
        }
        /// <summary>
        /// 根据账号获取角色id
        /// </summary>
        public long GetCharIdxByAccount(long account_idx)
        {
            long char_idx;
            if (m_account_players.TryGetValue(account_idx, out char_idx))
                return char_idx;
            return 0;
        }
        /// <summary>
        /// 根据角色id获取账号
        /// </summary>
        public long GetAccountByCharIdx(long char_idx)
        {
            Player player = this.GetPlayerByIdx(char_idx);
            if (player != null)
                return player.account_idx;
            return 0;
        }
        public int GetPlayerCount()
        {
            return m_players.Count;
        }
        public List<long> players
        {
            get { return m_players; }
        }
        #endregion
    }
}
