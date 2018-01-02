using System;
using System.Collections.Generic;
using System.Threading;

namespace dc
{
    /// <summary>
    /// cmd管理器
    /// @author hannibal
    /// @time 2016-9-20
    /// </summary>
    public class ServerCmdManager : Singleton<ServerCmdManager>
    {
        private delegate void CommanderCallBack(string[] param);

        private bool m_disposed = false;
        private Thread m_cmd_thread = null;
        private Dictionary<int, CommanderCallBack> m_cmd_callback = new Dictionary<int, CommanderCallBack>();
        private Dictionary<string, int> m_string2ids = new Dictionary<string, int>();

        public void Setup()
        {
            m_disposed = false;
            if (m_cmd_thread == null)
            {
                m_cmd_thread = new Thread(new ThreadStart(Update));
                m_cmd_thread.Priority = System.Threading.ThreadPriority.AboveNormal;
                m_cmd_thread.Start();
            }
            InsertCmdCallback(eCmdType.Help, OnCommand_Help);
            InsertCmdCallback(eCmdType.Clear, OnCommand_Clear);
            InsertCmdCallback(eCmdType.Reload, OnCommand_Reload);
            InsertCmdCallback(eCmdType.Shutdown, OnCommand_Shutdown);
            InsertCmdCallback(eCmdType.Pools, OnCommand_Pools);
            InsertCmdCallback(eCmdType.Online, OnCommand_Online);
            InsertCmdCallback(eCmdType.LogLv, OnCommand_LogLV);
        }
        public void Destroy()
        {
            m_disposed = true;
            if (m_cmd_thread != null)
            {
                try
                {
                    m_cmd_thread.Abort();
                }
                catch (Exception) { }
                m_cmd_thread = null;
            }
            m_cmd_callback.Clear();
            m_string2ids.Clear();
        }
        public void Update()
        {
            while (!m_disposed)
            {
                string content = Console.ReadLine();
                content = content.ToLower().Trim();

                do
                {
                    string[] lines = content.Split(' ');
                    if (lines != null && lines.Length > 0)
                    {
                        string cmd = lines[0];
                        int idx = 0;
                        if (!int.TryParse(cmd, out idx))
                        {
                            if (!m_string2ids.TryGetValue(cmd, out idx)) break;
                        }

                        CommanderCallBack callback;
                        if (!m_cmd_callback.TryGetValue(idx, out callback)) break;

                        lock (ThreadScheduler.Instance.LogicLock)
                        {//需要同步全局锁
                            try
                            {
                                callback(lines);
                            }
                            catch (Exception e)
                            {
                                Log.Exception(e);
                            }
                        }
                    }
                } while (false);

                Thread.Sleep(64);
            }
        }
        /// <summary>
        /// 添加执行命令
        /// </summary>
        private void InsertCmdCallback(eCmdType type, CommanderCallBack callback)
        {
            string cmd_str = cmd.list[(int)type];
            m_string2ids.Add(cmd_str, (int)type);
            m_cmd_callback.Add((int)type, callback);
        }
        /// <summary>
        /// 帮助
        /// </summary>
        private void OnCommand_Help(string[] param)
        {
            string tips = "输入下列数字或英文单词，按回车执行命令(如果需要带参数，以空格分隔；不区分大小写)\n";
            List<int> keys = new List<int>(m_cmd_callback.Keys);
            keys.Sort();
            foreach (var key in keys)
            {
                tips += (key + "." + cmd.list[key] + cmd.tips[key] + "\n");
            }
            Console.WriteLine(tips);
        }
        /// <summary>
        /// 清除控制台
        /// </summary>
        private void OnCommand_Clear(string[] param)
        {
            Console.Clear();
        }
        /// <summary>
        /// 重新加载配置表
        /// </summary>
        private void OnCommand_Reload(string[] param)
        {
            ConfigManager.Instance.UnloadAll();
            ConfigManager.Instance.LoadAll();
        }
        /// <summary>
        /// 关服
        /// </summary>
        private void OnCommand_Shutdown(string[] param)
        {
            int wait_time = param.Length >= 2 ? int.Parse(param[1]) : 0;
            Master.Instance.Stop(wait_time);
        }
        /// <summary>
        /// 对象池信息
        /// </summary>
        private void OnCommand_Pools(string[] param)
        {
            NetChannelPools.ToString(true);
            UserTokenPools.ToString(true);
            IOCPClientSocket.ToString(true);
            IOCPServerSocket.ToString(true);
            SendRecvBufferPools.ToString(true);
            PacketPools.ToString(true);
            ObjectPools.ToString(true);
            CommonObjectPools.ToString(true);
        }
        /// <summary>
        /// 在线人数统计
        /// </summary>
        private void OnCommand_Online(string[] param)
        {
            string str = "世界服在线人数:" + UnitManager.Instance.GetUnitCount();
            Console.WriteLine(str);
            string str_remote = ServerInfoManager.Instance.GetRemoteOnlineCount();
            Console.WriteLine("其他服连接人数:\n" + str_remote);
        }
        /// <summary>
        /// 设置日志等级
        /// </summary>
        private void OnCommand_LogLV(string[] param)
        {
            if (param.Length == 2 && StringUtils.IsInteger(param[1]))
            {
                eLogLevel lv = (eLogLevel)(int.Parse(param[1]));
                Log.SetLogLv(lv);
            }
        }
    }
}
