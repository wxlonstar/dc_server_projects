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
        private PlayerInfoForWS m_player_data = new PlayerInfoForWS();

        private long m_last_active_time = 0;        //上次活跃时间
        private bool m_is_send_check_online = false;//是否发给ss检测活跃

        public Unit()
        {
        }
        public void Init()
        {
            m_last_active_time = 0;
            m_is_send_check_online = false;
        }
        public void Setup(ClientUID client_uid, InterServerID srv_uid, PlayerInfoForWS info)
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

            //告诉客户端当前服务器时间
            ws2c.ServerTime rep_msg = PacketPools.Get(ws2c.msg.SERVER_TIME) as ws2c.ServerTime;
            rep_msg.server_time = GameTimeManager.Instance.server_time;
            ServerNetManager.Instance.SendProxy(m_client_uid, rep_msg);

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
            msg.char_idx = this.char_idx;
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

            if(is_online)
            {
                m_last_active_time = Time.timeSinceStartup;
            }
            else
            {
                UnitManager.Instance.HandleLogout(this.char_idx);
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
                case eUnitModType.UMT_char_type: m_player_data.char_type = (byte)value; break;
                case eUnitModType.UMT_flags: m_player_data.flags = (uint)value; break;
                case eUnitModType.UMT_model_idx: m_player_data.model_idx = (uint)value; break;
                case eUnitModType.UMT_job: m_player_data.job = (byte)value; break;
                case eUnitModType.UMT_level: m_player_data.level = (ushort)value; break;
                case eUnitModType.UMT_exp: m_player_data.exp = (uint)value; break;
                case eUnitModType.UMT_gold: m_player_data.gold = (uint)value; break;
                case eUnitModType.UMT_coin: m_player_data.coin = (uint)value; break;
                case eUnitModType.UMT_vip_grade: m_player_data.vip_grade = (uint)value; break;
                default: return;
            }
            m_last_active_time = Time.timeSinceStartup;
        }
        public void UpdateAttribute(eUnitModType type, string value)
        {
            switch (type)
            {
                case eUnitModType.UMT_char_name: m_player_data.char_name = value; break;
                default: return;
            }
            m_last_active_time = Time.timeSinceStartup;
        }
        public PlayerInfoForWS player_data
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
        public long account_idx
        {
            get { return m_player_data.account_idx; }
        }
        public long char_idx
        {
            get { return m_player_data.char_idx; }
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
