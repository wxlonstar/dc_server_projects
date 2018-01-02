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

            Log.Info("开启时间(" + DateTime.Now.ToString() + ")");
            ServerConfig.Load();

            Console.Title = "GL:" + ServerConfig.net_info.server_uid;

            Framework.Instance.Setup(Tick);
            DBManager.Instance.Setup();
            DataManager.Instance.Setup();
            UnitManager.Instance.Setup();
            ForServerNetManager.Instance.Setup();
            ServerCmdManager.Instance.Setup();
            RelationManager.Instance.Setup();
        }
        public void Destroy()
        {
            Framework.Instance.Destroy();
            DBManager.Instance.Destroy();
            DataManager.Instance.Destroy();
            UnitManager.Instance.Destroy();
            ForServerNetManager.Instance.Destroy();
            ServerCmdManager.Instance.Destroy();
            RelationManager.Instance.Destroy();

            Log.Info("服务器关闭");
        }
        public void Tick()
        {
            DBManager.Instance.Tick();
            UnitManager.Instance.Tick();
            ForServerNetManager.Instance.Tick();
            RelationManager.Instance.Tick();
        }

        public void Start()
        {
            DataManager.Instance.LoadAll();
            DBManager.Instance.Start(ServerConfig.db_info.db_list);
            ForServerNetManager.Instance.Start(ServerConfig.net_info.port_for_server);
            GameTimeManager.Instance.AddDayTimer(2017, 10, 29, 1, () => { Log.Info("触发日期"); });

            Framework.Instance.MainLoop();
        }
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
