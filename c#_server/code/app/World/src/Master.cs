using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 管理器
    /// @author hannibal
    /// @time 2016-7-28
    /// </summary>
    public class Master : Singleton<Master>
    {
        public void Setup()
        {
            //初始化日志，放最前面
            Log4Helper.Init(log4net.LogManager.GetLogger("loginfo"), log4net.LogManager.GetLogger("loginfo"), log4net.LogManager.GetLogger("logerror"), log4net.LogManager.GetLogger("logerror"));

            Log.Info("开启时间(" + DateTime.Now.ToString()+")");
            ServerConfig.Load();
            Console.Title = "WS:" + ServerConfig.net_info.server_uid;

            Framework.Instance.Setup(Tick);

            DataManager.Instance.Setup();
            DBManager.Instance.Setup();
            ServerCmdManager.Instance.Setup();
            ServerNetManager.Instance.Setup();
            UnitManager.Instance.Setup();
            AccountCacheManager.Instance.Setup();
        }
        public void Destroy()
        {
            Framework.Instance.Destroy();
            DataManager.Instance.Destroy();
            DBManager.Instance.Destroy();
            ServerCmdManager.Instance.Destroy();
            ServerNetManager.Instance.Destroy();
            UnitManager.Instance.Destroy();
            AccountCacheManager.Instance.Destroy();

            Log.Info("服务器关闭");
        }
        public void Tick()
        {
            DataManager.Instance.Tick();
            ServerNetManager.Instance.Tick();
            DBManager.Instance.Tick();
            UnitManager.Instance.Tick();
            AccountCacheManager.Instance.Tick();
        }

        public void Start()
        {
            DataManager.Instance.LoadAll();
            DBManager.Instance.Start(ServerConfig.db_info.db_list); 
            ServerNetManager.Instance.Start(ServerConfig.net_info.port_for_server);
            ServerNetManager.Instance.InitNextCharIdx();

            Framework.Instance.MainLoop();
        }
        #region 关服
        /// <summary>
        /// 主动关服
        /// </summary>
        public void Stop(int wait_time)
        {
            int leave_time = wait_time <= 0 ? GlobalID.TOTAL_WAIT_SHUTDOWN : wait_time;
            Log.Info("剩余关服时间:" + leave_time);

            //告诉客户端关服倒计时
            ws2c.ShutdownServer msg = PacketPools.Get(ws2c.msg.SHUTDOWN_SERVER) as ws2c.ShutdownServer;
            msg.leave_time = (ushort)leave_time;
            ServerNetManager.Instance.BroadcastProxyMsg(msg);

            //XX秒后自动关服，每秒触发一次
            TimerManager.Instance.AddLoop(1000, leave_time, (timer_id, param) =>
            {
                --leave_time;
                if (leave_time <= 0)
                    OnStopServer();
                else
                    Log.Info("剩余关服时间:" + leave_time);
            });
        }
        /// <summary>
        /// 执行关服逻辑
        /// </summary>
        private void OnStopServer()
        {
            inner.AppServerShutdown msg = PacketPools.Get(inner.msg.APPSERVER_SHUTDOWN) as inner.AppServerShutdown;
            ServerNetManager.Instance.BroadcastMsg(msg);

            //延迟一段时间销毁，等待其他app关闭
            TimerManager.Instance.AddOnce(3000, (timer_id, param) => 
            { 
                this.Destroy();

                while (true)
                {
                    System.Threading.Thread.Sleep(1);
                }
            });
        }
        #endregion
    }
}
