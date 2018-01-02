using System;
using System.Collections.Generic;

namespace dc
{
    public class ServerConfig
    {
        public static ServerNetInfo net_info;
        public static bool Load()
        {
            if (JsonFile.Read<ServerNetInfo>("./db.json", ref net_info))
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
        public string ip_for_server;//给其他服务器连接的ip
        public ushort port_for_server;//给其他服务器连接的port
        public List<DBItems> db_list = new List<DBItems>();//数据库列表
    }
}
