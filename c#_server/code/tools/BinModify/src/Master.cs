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

            DatabaseManager.Instance.Setup();
        }
        public void Destroy()
        {
            DatabaseManager.Instance.Destroy();

            Log.Info("服务器关闭");
        }
        public void Tick()
        {

        }

        public void Start()
        {
            DatabaseManager.Instance.Start();
        }
    }
}
