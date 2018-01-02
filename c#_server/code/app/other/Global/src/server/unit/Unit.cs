using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 玩家
    /// @author hannibal
    /// @time 2016-9-14
    /// </summary>
    public class Unit : IPoolsObject
    {
        private long m_char_idx = 0;
        private ushort m_ss_srv_uid = 0;        //所在ss服

        private bool m_is_online = false;
        private bool m_is_dirty = false;        //数据是否有更改
        private long m_last_access_time = 0;    //最后访问时间
        private long m_last_save_time = 0;      //最后保存时间
        private PlayerInfoForGL m_player_data = new PlayerInfoForGL();

        public Unit()
        {
        }
        public void Init()
        {
            m_char_idx = 0;
            m_is_online = false;
            m_is_dirty = false;
            m_last_access_time = 0;
            m_last_save_time = Time.second_time;
        }
        public void Setup(ushort uid, PlayerInfoForGL info)
        {
            m_char_idx = info.char_idx;
            m_ss_srv_uid = uid;
            m_player_data.Copy(info);
        }
        /// <summary>
        /// 上线
        /// </summary>
        public void OnEnter()
        {
            EventController.TriggerEvent(EventID.PLAYER_ENTER_GAME, m_char_idx);

            m_is_online = true;
            m_last_access_time = Time.second_time;
        }
        /// <summary>
        /// 下线
        /// </summary>
        public void OnLeave()
        {
            if (m_is_online)
            {
                //保存数据
                this.Save();

                EventController.TriggerEvent(EventID.PLAYER_LEAVE_GAME, m_char_idx);

                m_is_online = false;
                m_last_access_time = Time.second_time;
            }
        }
        public void Update()
        {
        }
        /// <summary>
        /// 保存到db
        /// </summary>
        public void Save()
        {
            SQLCharHandle.SaveCharacterInfo(m_player_data);

            m_is_dirty = false;
            m_last_save_time = Time.second_time;
        }
        /// <summary>
        /// 是否需要存盘
        /// </summary>
        /// <returns></returns>
        public bool NeedSave()
        {
            if (m_is_dirty && Time.second_time - m_last_save_time > 60 * 5)//5分钟保存一次
                return true;
            return false;
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        public void UpdateAttribute(eUnitModType type, long value)
        {
            switch (type)
            {
                case eUnitModType.UMT_char_type: m_player_data.char_type = (byte)value; break;
                case eUnitModType.UMT_flags: m_player_data.flags = (uint)value; break;
                case eUnitModType.UMT_scene_type: m_player_data.scene_type_idx = (uint)value; break;
                case eUnitModType.UMT_model_idx: m_player_data.model_idx = (uint)value; break;
                case eUnitModType.UMT_job: m_player_data.job = (byte)value; break;
                case eUnitModType.UMT_level: m_player_data.level = (ushort)value; break;
                case eUnitModType.UMT_exp: m_player_data.exp = (uint)value; break;
                case eUnitModType.UMT_gold: m_player_data.gold = (uint)value; break;
                case eUnitModType.UMT_coin: m_player_data.coin = (uint)value; break;
                case eUnitModType.UMT_vip_grade: m_player_data.vip_grade = (uint)value; break;
                default: return;
            }
            RelationManager.Instance.UpdateAttribute(m_char_idx, type, value);
            m_is_dirty = true;
            m_last_access_time = Time.second_time;
        }
        public void UpdateAttribute(eUnitModType type, string value)
        {
            switch (type)
            {
                case eUnitModType.UMT_char_name: m_player_data.char_name = value; break;
                default: return;
            }
            RelationManager.Instance.UpdateAttribute(m_char_idx, type, value);
            m_is_dirty = true;
            m_last_access_time = Time.second_time;
        }
        public bool is_online
        {
            get { return m_is_online; }
        }
        public long last_access_time
        {
            get { return m_last_access_time; }
        }
        public PlayerInfoForGL player_data
        {
            get { return m_player_data; }
        }
        public ushort ss_srv_uid
        {
            get { return m_ss_srv_uid; }
        }
        public long char_idx
        {
            get { return m_char_idx; }
        }
        public string char_name
        {
            get { return m_player_data.char_name; }
        }
        public byte char_type
        {
            get { return m_player_data.char_type; }
        }
        public ushort ws_id
        {
            get { return m_player_data.ws_id; }
        }
        public uint scene_type_idx
        {
            get { return m_player_data.scene_type_idx; }
        }
        public uint flags
        {
            get { return m_player_data.flags; }
        }
        public uint model_idx
        {
            get { return m_player_data.model_idx; }
        }
        public byte job
        {
            get { return m_player_data.job; }
        }
        public ushort level
        {
            get { return m_player_data.level; }
        }
        public uint exp
        {
            get { return m_player_data.exp; }
        }
        public uint gold
        {
            get { return m_player_data.gold; }
        }
        public uint coin
        {
            get { return m_player_data.coin; }
        }
        public uint vip_grade
        {
            get { return m_player_data.vip_grade; }
        }
    }
}
