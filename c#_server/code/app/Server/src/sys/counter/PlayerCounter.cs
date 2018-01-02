using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 玩家次数管理
    /// @author hannibal
    /// @time 2016-11-2
    /// </summary>
    public class PlayerCounter
    {
        private long m_char_idx = 0;
        private Dictionary<eCounterType, CounterInfo> m_counters = null;

        private long m_last_save_time = 0;      //最后保存时间

        public PlayerCounter()
        {
            m_counters = new Dictionary<eCounterType, CounterInfo>();
        }

        public void Setup(long char_idx)
        {
            m_char_idx = char_idx;
            //Load();
        }
        public void Destroy()
        {
            m_counters.Clear();
            m_char_idx = 0;
        }
        #region 加载
        /// <summary>
        /// 加载
        /// </summary>
        public void Load()
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null) return;
            
            SQLCounterHandle.QueryCounterList(m_char_idx, player.db_id, OnLoadedComplete);
        }
        private void OnLoadedComplete(bool is_load, ByteArray by)
        {                
            //查询过程如果下线
            if (m_char_idx == 0) return;
            if (is_load)
            {
                this.Derialize(by);
            }
            //判断是否初次登录，填充默认值
            this.InitCounter();

            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null) return;

            //下发给前端
            List<CounterInfo> list = new List<CounterInfo>();
            if (m_counters.Count > 0)
            {
                foreach (var obj in m_counters)
                {
                    list.Add(obj.Value);
                    if (list.Count == 50)
                    {//每次下发50个
                        ss2c.CounterList msg = PacketPools.Get(ss2c.msg.COUNTER_LIST) as ss2c.CounterList;
                        msg.list.AddRange(list);
                        ServerNetManager.Instance.SendProxy(player.client_uid, msg);

                        list.Clear();
                    }
                }
                if (list.Count > 0)
                {
                    ss2c.CounterList msg = PacketPools.Get(ss2c.msg.COUNTER_LIST) as ss2c.CounterList;
                    msg.list.AddRange(list);
                    ServerNetManager.Instance.SendProxy(player.client_uid, msg, false);
                }
            }
        }
        public void Save()
        {
            Player player = UnitManager.Instance.GetUnitByIdx(m_char_idx) as Player;
            if (player == null) return;

            ByteArray by = DBUtils.AllocDBArray();
            this.Serialize(by);
            SQLCounterHandle.UpdateCounter(m_char_idx, player.db_id, by);

            m_last_save_time = Time.timeSinceStartup;
        }
        /// <summary>
        /// 是否需要保存
        /// </summary>
        /// <returns></returns>
        public bool NeedSave()
        {
            if (Time.timeSinceStartup - m_last_save_time > 1000 * counter.DB_UPDATE_TIME_OFFSET)
                return true;
            return false;
        }
        #endregion

        #region 序列化
        public void Serialize(ByteArray by)
        {
            foreach (var obj in m_counters)
            {
                obj.Value.Write(by);
            }
        }
        public void Derialize(ByteArray by)
        {
            while (by.Available > 0 && by.Available >= RelationInfo.BaseSize)
            {
                CounterInfo info = new CounterInfo();
                info.Read(by);
                m_counters.Add(info.type, info);
            }
        }
        #endregion

        #region 数据管理
        /// <summary>
        /// 初始次数表，一般是新号或新功能开启
        /// </summary>
        private void InitCounter()
        {
            List<StdCounterInfo> list = new List<StdCounterInfo>();

            ///1.收集当前已经开启的
            Dictionary<int, StdCounterInfo> infos = CounterConfig.Instance.infos;
            foreach(var obj in infos)
            {
                //TODO：判断当前功能是否开启
                list.Add(obj.Value);
            }

            ///2.初始化列表信息
            foreach(var info in list)
            {
                CounterInfo counter_info;
                if(!m_counters.TryGetValue((eCounterType)info.idx, out counter_info))
                {
                    counter_info = new CounterInfo();
                    counter_info.type = (eCounterType)info.idx;
                    counter_info.count = this.GetMaxCounterByType((eCounterType)info.idx);
                    counter_info.cd_time = Time.second_time;
                    m_counters.Add((eCounterType)info.idx, counter_info);
                }
            }

            ///修正次数和cd
            List<CounterInfo> list_counter = new List<CounterInfo>(m_counters.Values);
            foreach (var obj in list_counter)
            {
                this.GetAndModifyCounter(obj.type);
            }
        }
        /// <summary>
        /// 减少次数
        /// </summary>
        public bool ConsumeCounter(eCounterType type, ushort modify_value)
        {
            this.GetAndModifyCounter(type);

            CounterInfo counter_info;
            if (m_counters.TryGetValue(type, out counter_info))
            {
                ushort max_count = GetMaxCounterByType(type);
                if(modify_value > max_count || max_count > counter_info.count)
                    return false;
                if(counter_info.count == max_count)
                {
                    counter_info.cd_time = Time.second_time;//如果是满次数的情况下扣次数，则从当前时间开始记cd
                }
                counter_info.count -= modify_value;
                m_counters[type] = counter_info;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取并修改次数
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ushort GetAndModifyCounter(eCounterType type)
        {
            if (!m_counters.ContainsKey(type)) return 0;

            long count = 0;//之所以定义long，防止下面计时溢出
            ushort max_count = GetMaxCounterByType(type);

            CounterInfo counter_info;
            if (m_counters.TryGetValue(type, out counter_info))
            {
                StdCounterInfo info = CounterConfig.Instance.GetInfo((int)type);
                if (info == null) return 0;

                switch((eCounterCDType)info.cd_type)
                {
                    case eCounterCDType.Without:
                        count = counter_info.count;
                        break;
                    case eCounterCDType.Fixed:
                        count = counter_info.count;
                        if (count < max_count)
                        {
                            //上次cd到现在经过的秒数
                            long offset_seconds = Time.second_time - counter_info.cd_time;
                            int cd_time = this.GetCDTimeByType(type);
                            long multiple_count = (long)(offset_seconds / (long)cd_time);
                            count += multiple_count;
                            if(count >= max_count)
                                counter_info.cd_time = Time.second_time;
                            else
                                counter_info.cd_time += multiple_count * cd_time;//修正开始cd时间
                        }
                        else
                            counter_info.cd_time = Time.second_time;
                        break;
                    case eCounterCDType.Increase:

                        break;
                    case eCounterCDType.Config:

                        break;
                }
            }

            //修正到有效次数
            if (count > max_count) count = max_count;
            counter_info.count = (ushort)count;
            //修改数据
            m_counters[type] = counter_info;

            return (ushort)count;
        }
        /// <summary>
        /// 获取类型最大次数，主要是考虑属性加成或等级加成
        /// </summary>
        public ushort GetMaxCounterByType(eCounterType type)
        {
            StdCounterInfo info = CounterConfig.Instance.GetInfo((int)type);
            if (info == null) return 0;

            //TODO:属性加成

            return info.value;
        }
        /// <summary>
        /// 获取需要的cd
        /// </summary>
        public int GetCDTimeByType(eCounterType type)
        {
            StdCounterInfo info = CounterConfig.Instance.GetInfo((int)type);
            if (info == null) return 0;

            int cd_time = 0;
            switch ((eCounterCDType)info.cd_type)
            {
                case eCounterCDType.Without:
                    break;
                case eCounterCDType.Fixed:
                    cd_time = info.cd_value[0];
                    break;
                case eCounterCDType.Increase:
                    //TODO
                    break;
                case eCounterCDType.Config:
                    //TODO
                    break;
            }

            return cd_time;
        }
        #endregion
    }
}
