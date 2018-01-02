using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 客户端会话
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class ClientSession : IPoolsObject
    {
        private long m_conn_idx = 0;
        private long m_account_idx = 0;
        private ushort m_spid = 0;          //所在渠道id
        private ushort m_ss_uid = 0;        //所在ss
        private ushort m_fs_uid = 0;        //所在fs
        private DBID m_db_id;               //所在db
        private ClientUID m_client_uid;     //客户端在gate的id

        private float m_conn_time = 0;      //连上时间
        private byte m_login_error_count = 0;//登录错误次数

        private uint m_last_packet_idx = 0; //做数据校验用

        private eSessionStatus m_session_status = eSessionStatus.INVALID;

        private PacketCDVerify m_cooldown_verify = new PacketCDVerify();    //消息cd校验
        private SpeedChecker m_speed_checker = new SpeedChecker();          //防加速

        public void Init()
        {

        }
        public void Setup(long conn_idx)
        {
            m_conn_idx = conn_idx;
            m_account_idx = 0;
            m_spid = 0;
            m_fs_uid = 0;
            m_ss_uid = 0;
            m_db_id.Reset();
            m_client_uid.conn_idx = conn_idx;
            m_client_uid.srv_uid = ServerNetManager.Instance.srv_uid;

            m_conn_time = Time.timeSinceStartup;
            m_login_error_count = 0;
            m_last_packet_idx = 0;

            m_cooldown_verify.Reset();
            m_speed_checker.Reset(m_conn_idx);
            m_session_status = eSessionStatus.CREATED;
        }
        public void Destroy()
        {
            m_conn_idx = 0;
            m_fs_uid = 0;
            m_ss_uid = 0;
            m_db_id.Reset();
        }

        public void Update()
        {
            if(m_session_status == eSessionStatus.IN_GAMING)
            {
                m_speed_checker.Update();
            }
        }
        /// <summary>
        /// 发给客户端
        /// </summary>
        public int Send(PacketBase packet)
        {
            return ForClientNetManager.Instance.Send(m_conn_idx, packet);
        }
        /// <summary>
        /// 发给ss
        /// </summary>
        public int Send2SS(PacketBase packet)
        {
            if (m_ss_uid == 0)
                return 0;
            return ForServerNetManager.Instance.Send(m_ss_uid, packet);
        }
        /// <summary>
        /// 主动登出，或异常退出
        /// </summary>
        public void	Logout()
        {
            if (m_session_status == eSessionStatus.LOGOUTING) return;
            m_session_status = eSessionStatus.LOGOUTING;

            //发送到ss,登出
            if(m_account_idx > 0)
            {
                gs2ss.LogoutAccount ss_msg = PacketPools.Get(gs2ss.msg.LOGOUT_ACCOUNT) as gs2ss.LogoutAccount;
                ss_msg.account_idx = m_account_idx;
                this.Send2SS(ss_msg);
            }
        }

        /// <summary>
        /// 被踢出
        /// </summary>
        public void Kickout()
        {
            if (m_session_status == eSessionStatus.LOGOUTING) return;
            m_session_status = eSessionStatus.LOGOUTING;

            //已经是正式连接的客户端，需要做后续清理工作
            if (m_account_idx > 0)
            {//说明已经登录到ss，需要清理
                gs2ss.KickoutAccount msg = PacketPools.Get(gs2ss.msg.KICK_ACCOUNT) as gs2ss.KickoutAccount;
                msg.account_idx = m_account_idx;
                this.Send2SS(msg);
            }
        }
        public bool CheckCooldown(ushort msg_idx)
        {
            return m_cooldown_verify.CheckCooldown(msg_idx);
        }
        public long conn_idx
        {
            get { return m_conn_idx; }
        }
        public ClientUID client_uid
        {
            get { return m_client_uid; }
        }
        public long account_idx
        {
            get { return m_account_idx; }
            set 
            { 
                m_account_idx = value;
                m_db_id.game_id = ServerConfig.GetDBByAccountIdx(m_account_idx, eDBType.Game);
                m_db_id.center_id = ServerConfig.GetDBByAccountIdx(m_account_idx, eDBType.Center);
                m_db_id.log_id = ServerConfig.GetDBByAccountIdx(m_account_idx, eDBType.Log);
            }
        }
        public byte login_error_count
        {
            get { return m_login_error_count; }
            set { m_login_error_count = value; }
        }
        public uint last_packet_idx
        {
            get { return m_last_packet_idx; }
            set { m_last_packet_idx = value; }
        }
        public ushort spid
        {
            get { return m_spid; }
            set { m_spid = value; }
        }
        public ushort ss_uid
        {
            get { return m_ss_uid; }
            set { m_ss_uid = value; }
        }
        public ushort fs_uid
        {
            get { return m_fs_uid; }
            set { m_fs_uid = value; }
        }
        public DBID db_id
        {
            get { return m_db_id; }
        }
        public eSessionStatus session_status
        {
            get { return m_session_status; }
            set { m_session_status = value; }
        }
        public SpeedChecker speed_checker
        {
            get { return m_speed_checker; }
        }
    }
    /// <summary>
    /// sessioin状态
    /// </summary>
    public enum eSessionStatus
    {
        INVALID = 0,			// 非法值
        CREATED,	            // 已创建
        LOGIN_DOING,	        // 登录中
        ALREADY_LOGIN,	        // 登录成功
        LOGIN_FAILED,	        // 登录失败
        ENTER_GAMING,	        // 请求进入游戏中
        IN_GAMING,	            // 游戏中
        DELAY_DISCONNECT,	    // 延迟断线
        LOGOUTING,				// 登出中
    }
}
