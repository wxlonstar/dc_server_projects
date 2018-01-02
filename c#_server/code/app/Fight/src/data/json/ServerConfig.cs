using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 服务器配置
    /// @author hannibal
    /// @time 2016-8-1
    /// </summary>
    public class ServerConfig
    {
        public static ServerNetInfo net_info;
        public static bool Load()
        {
            if (JsonFile.Read<ServerNetInfo>("./fight.json", ref net_info))
                return true;
            else 
                return false;
        }
    }

    public class ServerNetInfo
    {
        public int log_level;
        public ushort server_uid;//本区服务器唯一id
        public string ws_ip;//ws ip
        public ushort ws_port;//ws port
        public string ip_for_server;
        public ushort port_for_server;
    }
}
