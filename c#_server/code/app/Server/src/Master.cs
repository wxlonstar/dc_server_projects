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

            Log.Info("开启时间(" + DateTime.Now.ToString() + ")");
            ServerConfig.Load();
            Console.Title = "SS:" + ServerConfig.net_info.server_uid;

            Framework.Instance.Setup(Tick);
            DataManager.Instance.Setup();
            DBManager.Instance.Setup();
            ServerNetManager.Instance.Setup();
            SceneManager.Instance.Setup();
            UnitManager.Instance.Setup();
            AOIManager.Instance.Setup();
            ServerCmdManager.Instance.Setup();
            DBEventManager.Instance.Setup();
            CounterManager.Instance.Setup();
            MailboxManager.Instance.Setup();
            RelationManager.Instance.Setup();
            ChatManager.Instance.Setup();
            FightManager.Instance.Setup();
        }
        public void Destroy()
        {
            Framework.Instance.Destroy();
            DataManager.Instance.Destroy();
            DBManager.Instance.Destroy();
            ServerNetManager.Instance.Destroy();
            SceneManager.Instance.Destroy();
            UnitManager.Instance.Destroy();
            AOIManager.Instance.Destroy();
            ServerCmdManager.Instance.Destroy();
            DBEventManager.Instance.Destroy();
            CounterManager.Instance.Destroy();
            MailboxManager.Instance.Destroy();
            RelationManager.Instance.Destroy();
            ChatManager.Instance.Destroy();
            FightManager.Instance.Destroy();

            Log.Info("服务器关闭");
        }
        public void Tick()
        {
            ServerNetManager.Instance.Tick();
            DBManager.Instance.Tick();
            UnitManager.Instance.Tick();
            AOIManager.Instance.Tick();
            DBEventManager.Instance.Tick();
            CounterManager.Instance.Tick();
            MailboxManager.Instance.Tick();
            RelationManager.Instance.Tick();
            ChatManager.Instance.Tick();
            FightManager.Instance.Tick();
        }

        public void Start()
        {
            DataManager.Instance.LoadAll();
            DBManager.Instance.Start(ServerConfig.db_info.db_list); 
            ServerNetManager.Instance.Connect2WorldServer(ServerConfig.net_info.ws_ip, ServerConfig.net_info.ws_port);

            DBID db_id = new DBID();
            db_id.game_id = 100;

            DBEventInfo e_info = new DBEventInfo();
            e_info.target_char_idx = 10000000002;
            e_info.source_char_idx = 10000000003;
            e_info.event_type = eDBEventType.Test;
            e_info.bin_content.bin_normal_content.data = "测试了";
            SQLDBEventHandle.InsertDBEvent(e_info, db_id);

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
