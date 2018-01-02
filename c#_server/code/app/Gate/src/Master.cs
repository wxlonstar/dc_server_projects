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
            Log4Helper.Init(log4net.LogManager.GetLogger("loginfo"),log4net.LogManager.GetLogger("loginfo"),log4net.LogManager.GetLogger("logerror"),log4net.LogManager.GetLogger("logerror"));

            //CSVDocument doc = new CSVDocument();
            //doc.Load(@"..\data\config\skill\StdSpellInfo.csv");

            Log.Info("开启时间(" + DateTime.Now.ToString() + ")");
            ServerConfig.Load();

            Console.Title = "GS:" + ServerConfig.net_info.server_uid;

            Framework.Instance.Setup(Tick);
            DataManager.Instance.Setup();
            DBManager.Instance.Setup();
            ServerNetManager.Instance.Setup();
            ForClientNetManager.Instance.Setup();
            ForServerNetManager.Instance.Setup();
            ClientSessionManager.Instance.Setup();
            ServerCmdManager.Instance.Setup();
        }
        public void Destroy()
        {
            Framework.Instance.Destroy();
            DataManager.Instance.Destroy();
            DBManager.Instance.Destroy();
            ServerNetManager.Instance.Destroy();
            ForClientNetManager.Instance.Destroy();
            ForServerNetManager.Instance.Destroy();
            ClientSessionManager.Instance.Destroy();
            ServerCmdManager.Instance.Destroy();

            Log.Info("服务器关闭");
        }
        public void Tick()
        {
            ServerNetManager.Instance.Tick();
            DBManager.Instance.Tick();
            ForClientNetManager.Instance.Tick();
            ForServerNetManager.Instance.Tick();
            ClientSessionManager.Instance.Tick();
        }

        public void Start()
        {
            DataManager.Instance.LoadAll();
            ServerNetManager.Instance.Connect2WorldServer(ServerConfig.net_info.ws_ip, ServerConfig.net_info.ws_port);
            Framework.Instance.MainLoop();
        }
        /// <summary>
        /// 主动关服
        /// </summary>
        public void Stop()
        {
            this.Destroy();

            while (true)
            {
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
