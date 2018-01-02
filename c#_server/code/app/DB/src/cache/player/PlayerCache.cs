using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 单个玩家缓存
    /// @author hannibal
    /// @time 2017-9-7
    /// </summary>
    public class PlayerCache : IPoolsObject
    {
        private long m_char_idx;
        private ClientUID m_client_uid;         //客户端id
        private ushort m_ss_uid;                //所在ss服务器id

        private bool m_is_dirty = false;        //数据是否有更改
        private long m_last_access_time = 0;    //最后访问时间
        private long m_last_save_time = 0;      //最后保存时间
        private PlayerInfoForSS m_ss_data = null;

        public PlayerCache()
        {
            m_ss_data = CommonObjectPools.Spawn<PlayerInfoForSS>();
        }
        /// <summary>
        /// 对象池初始化
        /// </summary>
        public void Init()
        {
            m_is_dirty = false;
            m_last_access_time = 0;
            m_last_save_time = Time.timeSinceStartup;
        }
        /// <summary>
        /// 从db加载数据
        /// </summary>
        /// <param name="_char_idx"></param>
        /// <returns></returns>
        public void Load(long _char_idx, Action<bool> callback)
        {
            m_char_idx = _char_idx;
            SQLCharHandle.QueryCharacterInfo(m_char_idx, m_ss_data, is_load => 
            {
                callback(is_load);
            });
        }
        /// <summary>
        /// 保存到db
        /// </summary>
        public void Save()
        {
            SQLCharHandle.SaveCharacterInfo(m_ss_data);

            m_is_dirty = false;
            m_last_save_time = Time.timeSinceStartup;
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public void UpdateAttribute(eUnitModType type, long value)
        {
            switch (type)
            {
                case eUnitModType.UMT_char_type: m_ss_data.char_type = (byte)value; break;
                case eUnitModType.UMT_flags: m_ss_data.flags = (uint)value; break;
                case eUnitModType.UMT_model_idx: m_ss_data.model_idx = (uint)value; break;
                case eUnitModType.UMT_job: m_ss_data.job = (byte)value; break;
                case eUnitModType.UMT_level: m_ss_data.level = (ushort)value; break;
                case eUnitModType.UMT_exp: m_ss_data.exp = (uint)value; break;
                case eUnitModType.UMT_energy: m_ss_data.energy = (uint)value; break;
                case eUnitModType.UMT_gold: m_ss_data.gold = (uint)value; break;
                case eUnitModType.UMT_coin: m_ss_data.coin = (uint)value; break;
                case eUnitModType.UMT_hp: m_ss_data.hp = (uint)value; break;
                case eUnitModType.UMT_vip_grade: m_ss_data.vip_grade = (uint)value; break;
                case eUnitModType.UMT_vip_flags: m_ss_data.vip_flags = (uint)value; break;
                case eUnitModType.UMT_time_last_login: m_ss_data.time_last_login = value; break;
                case eUnitModType.UMT_time_last_logout: m_ss_data.time_last_logout = value; break;

                case eUnitModType.UMT_base_energy: m_ss_data.energy = (uint)value; break;
                case eUnitModType.UMT_base_hurt: m_ss_data.hurt = (uint)value; break;
                case eUnitModType.UMT_base_run_speed: m_ss_data.run_speed = (uint)value; break;

                case eUnitModType.UMT_hp_max: m_ss_data.hp_max = (uint)value; break;
                case eUnitModType.UMT_hurt: m_ss_data.hurt = (uint)value; break;
                case eUnitModType.UMT_range: m_ss_data.range = (uint)value; break;
                case eUnitModType.UMT_run_speed: m_ss_data.run_speed = (uint)value; break;
                default: return;
            }
            m_is_dirty = true;
            m_last_access_time = Time.timeSinceStartup;
        }
        public void UpdateAttribute(eUnitModType type, string value)
        {
            switch (type)
            {
                case eUnitModType.UMT_char_name: m_ss_data.char_name = value; break;
                default: return;
            }
            m_is_dirty = true;
            m_last_access_time = Time.timeSinceStartup;
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
        public long char_idx
        {
            get { return m_char_idx; }
            set { m_char_idx = value; }
        }
        public ClientUID client_uid
        {
            get { return m_client_uid; }
            set { m_client_uid = value; }
        }
        public ushort ss_uid
        {
            get { return m_ss_uid; }
            set { m_ss_uid = value; }
        }
        public long last_access_time
        {
            get { return m_last_access_time; }
        }
        public PlayerInfoForSS ss_data
        {
            get { return m_ss_data; }
        }
    }
}
