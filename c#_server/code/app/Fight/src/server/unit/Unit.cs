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
        private ClientUID m_client_uid;             //所在client，gate     
        private InterServerID m_srv_uid;            //所在ss服
        private PlayerInfoForFS m_player_data = new PlayerInfoForFS();

        private long m_last_active_time = 0;        //上次活跃时间
        private bool m_is_send_check_online = false;//是否发给ss检测活跃

        public Unit()
        {
        }
        public void Init()
        {
            m_char_idx = 0;
        }
        public void Setup(ClientUID client_uid, InterServerID srv_uid, PlayerInfoForFS info)
        {
            m_char_idx = info.char_idx;
            m_client_uid = client_uid;
            m_srv_uid = srv_uid;
            m_player_data.Copy(info);
        }
        /// <summary>
        /// 上线
        /// </summary>
        public void OnEnter()
        {
            m_last_active_time = Time.timeSinceStartup;
            m_is_send_check_online = false;
            EventController.TriggerEvent(EventID.PLAYER_ENTER_GAME, m_char_idx);
        }
        /// <summary>
        /// 下线
        /// </summary>
        public void OnLeave()
        {
            EventController.TriggerEvent(EventID.PLAYER_LEAVE_GAME, m_char_idx);
        }
        public void Update()
        {
            this.UpdateCheckOnline();
        }

        #region 检测是否在线
        private void UpdateCheckOnline()
        {
            //超过时间没有活跃，检测是否还在线
            if (!m_is_send_check_online && Time.timeSinceStartup - m_last_active_time >= GlobalID.TOTAL_CHECK_ONLINE_TIME * 1000)
            {
                this.UpdateCheckOnline();
            }
        }
        /// <summary>
        /// 发给ss检测
        /// </summary>
        private void SendCheckOnline()
        {
            ws2ss.ClientOnline msg = PacketPools.Get(ws2ss.msg.CLIENT_ONLINE) as ws2ss.ClientOnline;
            msg.char_idx = m_char_idx;
            ServerNetManager.Instance.Send(m_srv_uid.ss_uid, msg);

            m_is_send_check_online = true;
        }
        /// <summary>
        /// 收到ss反馈
        /// </summary>
        /// <param name="is_online"></param>
        public void RecvCheckOnline(bool is_online)
        {
            if (!m_is_send_check_online) return;
            m_is_send_check_online = false;

            if (is_online)
            {
                m_last_active_time = Time.timeSinceStartup;
            }
            else
            {
                UnitManager.Instance.HandleLogout(m_char_idx);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 属性改变
        /// </summary>
        public void UpdateAttribute(eUnitModType type, long value)
        {
            switch (type)
            {
                default: return;
            }
            //m_last_active_time = Time.timeSinceStartup;
        }
        public void UpdateAttribute(eUnitModType type, string value)
        {
            switch (type)
            {
                default: return;
            }
            //m_last_active_time = Time.timeSinceStartup;
        }
        public PlayerInfoForFS player_data
        {
            get { return m_player_data; }
        }
        public ClientUID client_uid
        {
            get { return m_client_uid; }
        }
        public ushort ss_srv_uid
        {
            get { return m_srv_uid.ss_uid; }
        }
        public ushort gs_srv_uid
        {
            get { return m_srv_uid.gs_uid; }
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
        #endregion
    }
}
