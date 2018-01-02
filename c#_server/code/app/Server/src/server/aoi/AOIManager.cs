using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// unit aoi 管理器
    /// @author hannibal
    /// @time 2016-9-1
    /// </summary>
    public class AOIManager : Singleton<AOIManager>
    {
        private Dictionary<long, AOIUnitInfo> m_aoi_units;
        public AOIManager()
        {
            m_aoi_units = new Dictionary<long, AOIUnitInfo>();
        }

        public void Setup()
        {
        }
        public void Destroy()
        {
            foreach(var obj in m_aoi_units)
            {
                CommonObjectPools.Despawn(obj.Value);
            }
            m_aoi_units.Clear();
        }
        public void Tick()
        {
        }
        /// <summary>
        /// 添加aoi对象
        /// </summary>
        /// <param name="char_idx"></param>
        /// <param name="scene_idx"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void Add(long char_idx, long scene_idx, int row, int col)
        {
            AOIUnitInfo aoi_info = null;
            if (!m_aoi_units.TryGetValue(char_idx, out aoi_info))
            {
                aoi_info = CommonObjectPools.Spawn<AOIUnitInfo>();
                aoi_info.char_idx = char_idx;
                aoi_info.scene_idx = scene_idx;
                m_aoi_units.Add(char_idx, aoi_info);
            }
            UpdatePosition(char_idx, row, col, true);
        }
        /// <summary>
        /// 可能是下线，或是离开场景
        /// </summary>
        /// <param name="char_idx"></param>
        public void Remove(long char_idx)
        {
            AOIUnitInfo aoi_info = null;
            if (!m_aoi_units.TryGetValue(char_idx, out aoi_info))
                return;

            ///广播离开事件
            AOIUnitInfo info = null;
            for (int i = aoi_info.observer_units.Count - 1; i >= 0; i--)
            {
                long idx = aoi_info.observer_units[i];
                if (m_aoi_units.TryGetValue(idx, out info))
                {
                    info.observer_units.Remove(aoi_info.char_idx);//从对方列表删除
                    SendAOILeave(info.char_idx, aoi_info.char_idx);
                }
            }

            CommonObjectPools.Despawn(aoi_info);
            m_aoi_units.Remove(char_idx);
        }
        /// <summary>
        /// 获取同屏unit
        /// </summary>
        /// <param name="char_idx"></param>
        /// <returns></returns>
        public List<long> GetScreenUnit(long char_idx)
        {
            AOIUnitInfo aoi_info = null;
            if (!m_aoi_units.TryGetValue(char_idx, out aoi_info))
                return null;
            return aoi_info.observer_units;
        }
        /// <summary>
        /// aoi对象移动
        /// </summary>
        /// <param name="char_idx"></param>
        /// <param name="row">对应y坐标</param>
        /// <param name="col">对应x坐标</param>
        public void UpdatePosition(long char_idx, int row, int col, bool is_add)
        {
            AOIUnitInfo info = null;
            AOIUnitInfo aoi_info = null;
            if(!m_aoi_units.TryGetValue(char_idx, out aoi_info))
            {
                return;
            }
            aoi_info.row = row;
            aoi_info.col = col;

            ///1.判断进入的
            foreach(var obj in m_aoi_units)
            {
                info = obj.Value;
                //是否在同一个场景
                if (info.scene_idx != aoi_info.scene_idx) continue;
                //排除自己
                if (info.char_idx == aoi_info.char_idx) continue;
                //排除已经在aoi列表的
                if (aoi_info.observer_units.Contains(info.char_idx)) continue;
                if (IsInRegion(aoi_info.row, aoi_info.col, info.row, info.col))
                {//进入区域
                    aoi_info.observer_units.Add(info.char_idx);//加入己方列表
                    info.observer_units.Add(aoi_info.char_idx);//加入对方列表

                    //通知进入
                    SendAOIEnter(aoi_info.char_idx, info.char_idx, info.row, info.col);
                    SendAOIEnter(info.char_idx, aoi_info.char_idx, aoi_info.row, aoi_info.col);
                }
            }

            ///2.判断离开的
            for (int i = aoi_info.observer_units.Count - 1; i >= 0; i--)
            {
                long idx = aoi_info.observer_units[i];
                if (m_aoi_units.TryGetValue(idx, out info))
                {
                    if(!IsInRegion(aoi_info.row, aoi_info.col, info.row, info.col))
                    {//离开区域
                        aoi_info.observer_units.RemoveAt(i);//从己方列表删除
                        info.observer_units.Remove(aoi_info.char_idx);//从对方列表删除

                        //通知离开
                        SendAOILeave(aoi_info.char_idx, info.char_idx);
                        SendAOILeave(info.char_idx, aoi_info.char_idx);
                    }
                }
                else
                {//不在列表，可能unit已经不存在，删除引用关系
                    aoi_info.observer_units.RemoveAt(i);
                }
            }

            ///3.给当前aoi区域的对象广播位置
            if (!is_add)
            {
                for (int i = aoi_info.observer_units.Count - 1; i >= 0; i--)
                {
                    long idx = aoi_info.observer_units[i];
                    if (m_aoi_units.TryGetValue(idx, out info))
                    {
                        SendAOIMove(info.char_idx, aoi_info.char_idx, row, col);
                    }
                }
            }
        }
        /// <summary>
        /// 是否在aoi区域
        /// </summary>
        /// <param name="center_row"></param>
        /// <param name="center_col"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private bool IsInRegion(int center_row, int center_col, int row, int col)
        {
            if (row >= center_row - aoi.REGION_ROWS && row <= center_row + aoi.REGION_ROWS
                && col >= center_col - aoi.REGION_COLS && col <= center_col + aoi.REGION_COLS)
                return true;
            return false;
        }

        private void SendAOIEnter(long char_idx, long enter_unit_idx, int row, int col)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(char_idx) as Player;
            if (player == null) return;
            Unit enter_unit = UnitManager.Instance.GetUnitByIdx(enter_unit_idx);
            if (enter_unit == null) return;

            //广播aoi消息
            ss2c.UnitEnterAOI rep_msg = PacketPools.Get(ss2c.msg.ENTER_AOI) as ss2c.UnitEnterAOI;
            rep_msg.unit_idx.Set(enter_unit.unit_type, 0, enter_unit.obj_idx);
            rep_msg.pos = new Position2D(col, row);
            rep_msg.dir = eDirection.TOP;
            rep_msg.flags = 0;
            rep_msg.unit_info = enter_unit.GetUnitAOIInfo();
            ServerNetManager.Instance.SendProxy(player.client_uid, rep_msg, false);
        }
        private void SendAOILeave(long char_idx, long leave_unit_idx)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(char_idx) as Player;
            if (player == null) return;
            Unit leave_unit = UnitManager.Instance.GetUnitByIdx(leave_unit_idx);
            if (leave_unit == null) return;

            ss2c.UnitLeaveAOI rep_msg = PacketPools.Get(ss2c.msg.LEAVE_AOI) as ss2c.UnitLeaveAOI;
            rep_msg.unit_idx.Set(leave_unit.unit_type, 0, leave_unit.obj_idx);
            rep_msg.flags = 0;
            ServerNetManager.Instance.SendProxy(player.client_uid, rep_msg, false);
        }
        private void SendAOIMove(long char_idx, long move_unit_idx, int row, int col)
        {
            Player player = UnitManager.Instance.GetUnitByIdx(char_idx) as Player;
            if (player == null) return;
            Unit move_unit = UnitManager.Instance.GetUnitByIdx(move_unit_idx);
            if (move_unit == null) return;

            ss2c.UnitMove rep_msg = PacketPools.Get(ss2c.msg.UNIT_MOVE) as ss2c.UnitMove;
            rep_msg.unit_idx.Set(move_unit.unit_type, 0, move_unit.obj_idx);
            rep_msg.pos.Set(col, row);
            rep_msg.flags = 0;
            ServerNetManager.Instance.SendProxy(player.client_uid, rep_msg, false);
            PacketPools.Recover(rep_msg);
        }
    }
}
