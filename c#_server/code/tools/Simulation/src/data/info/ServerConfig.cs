using System;
using System.Collections.Generic;

namespace dc
{
    public class ServerConfig
    {
        public static ServerNetInfo net_info;
        public static bool Read()
        {
            if (JsonFile.Read<ServerNetInfo>("./simulation.json", ref net_info))
                return true;
            else 
                return false;
        }
    }

    public class ServerNetInfo
    {
        //是否显示小黑窗
        public byte console;
        //压测
        public string net_server_ip;
        public ushort net_server_port;
        public string user_name;
        public string user_psw;
    }
}
