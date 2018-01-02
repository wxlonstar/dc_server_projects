using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 处于战斗中的玩家
    /// 1.玩家每次进入战场，会创建一个FightPlayer；退出战场是清除
    /// @author hannibal
    /// @time 2017-8-14
    /// </summary>
    public class FightPlayer : IPoolsObject
    {
        private long m_char_idx = 0;
        private eFightStage m_fight_stage = eFightStage.None;
        private eFSConnectState m_connect_state = eFSConnectState.UnConnect;

        public FightPlayer()
        {
        }
        /// <summary>
        /// 对象池初始化
        /// </summary>
        public void Init()
        {
            m_char_idx = 0;
            m_fight_stage = eFightStage.None;
            m_connect_state = eFSConnectState.UnConnect;
        }
        public void Setup(long _char_idx)
        {
            m_char_idx = _char_idx;

            //分配战斗服
            Player player = UnitManager.Instance.GetPlayerByIdx(_char_idx);
            if (player != null && player.fs_uid == 0)
            {
                //新进战场，分配战斗服id
                player.fs_uid = ServerNetManager.Instance.AllocFightServer();
            }
        }
        public void Destroy()
        {
            this.DisConnectFightServer();
            m_char_idx = 0;
        }
        public void Update()
        {
            if (m_connect_state != eFSConnectState.Connected)
            {
                this.ReconnectFightServer();
            }
        }
        /// <summary>
        /// 进入战场
        /// </summary>
        public void EnterFight()
        {
            m_fight_stage = eFightStage.Match;
            this.ConnectFightServer();
        }
        /// <summary>
        /// 退出战场
        /// </summary>
        public void LeaveFight()
        {
            this.DisConnectFightServer();

            Player player = UnitManager.Instance.GetPlayerByIdx(m_char_idx);
            if (player != null)
            {
                player.fs_uid = 0;
            }
            m_fight_stage = eFightStage.Finish;
        }

        #region 战斗服连接
        /// <summary>
        /// 连接战斗服：战斗服只在需要的时候连接，如匹配，战斗中等
        /// </summary>
        public void ConnectFightServer()
        {
            //已经连接了战斗服，直接返回
            if (m_connect_state == eFSConnectState.Connected)
                return;

            Player player = UnitManager.Instance.GetPlayerByIdx(m_char_idx);
            if (player == null || player.fs_uid == 0) 
                return;

            //判断是否已经有战斗服
            ConnAppProc app_server = ServerNetManager.Instance.GetConnApp(player.fs_uid);
            if (app_server == null || app_server.srv_info.srv_status != eConnAppStatus.CONNECTED)
                return;

            //如果存在，就当已经连上，因为本身ss已经连上fs
            {
                //告诉fs
                ss2fs.LoginClient msg = PacketPools.Get(ss2fs.msg.LOGIN_CLIENT) as ss2fs.LoginClient;
                msg.client_uid = player.client_uid;
                msg.data.Copy(player.unit_attr.player_info);
                ServerNetManager.Instance.Send2FS(player.fs_uid, msg);

                //告诉gs分配给玩家的fs
                ss2gs.NotifyServer gs_msg = PacketPools.Get(ss2gs.msg.NOTIFY_SERVER) as ss2gs.NotifyServer;
                gs_msg.account_idx = player.account_idx;
                gs_msg.s_type = eServerType.FIGHT;
                gs_msg.fs_uid = player.fs_uid;
                ServerNetManager.Instance.Send(player.client_uid.srv_uid, gs_msg);

                m_connect_state = eFSConnectState.Connected;
            }
        }
        /// <summary>
        /// 断开与战斗服的连接
        /// </summary>
        public void DisConnectFightServer()
        {
            Player player = UnitManager.Instance.GetPlayerByIdx(m_char_idx);
            if (player == null) return;

            if (player.fs_uid > 0 && m_connect_state == eFSConnectState.Connected)
            {
                //告诉fs
                ss2fs.LogoutClient fs_msg = PacketPools.Get(ss2fs.msg.LOGOUT_CLIENT) as ss2fs.LogoutClient;
                fs_msg.char_idx = m_char_idx;
                ServerNetManager.Instance.Send2FS(player.fs_uid, fs_msg);

                //告诉gs分配给玩家的fs
                ss2gs.NotifyServer gs_msg = PacketPools.Get(ss2gs.msg.NOTIFY_SERVER) as ss2gs.NotifyServer;
                gs_msg.account_idx = player.account_idx;
                gs_msg.s_type = eServerType.FIGHT;
                gs_msg.fs_uid = 0;
                ServerNetManager.Instance.Send(player.client_uid.srv_uid, gs_msg);

                m_connect_state = eFSConnectState.UnConnect;
            }
        }
        /// <summary>
        /// 重连战斗服：战斗过程中断线时
        /// </summary>
        private long lastConnectTime = 0;
        public void ReconnectFightServer()
        {
            if (Time.timeSinceStartup - lastConnectTime >= 1000)
            {
                this.ConnectFightServer();
                lastConnectTime = Time.timeSinceStartup;
            }
        }
        /// <summary>
        /// 战斗服退出
        /// </summary>
        public void HandleFightServerLeave(ushort uid)
        {
            Player player = UnitManager.Instance.GetPlayerByIdx(m_char_idx);
            if (player == null) return;

            if (player.fs_uid != uid) return;

            //TODO:如果当前正在战斗中，执行退出逻辑

            player.fs_uid = 0;
        }
        #endregion
        /// <summary>
        /// 设置状态
        /// </summary>
        public void SetStage(eFightStage stage)
        {
            m_fight_stage = stage;
        }
        public eFightStage fight_stage
        {
            get { return m_fight_stage; }
        }
    }
}
