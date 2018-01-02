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
        public static ServerDBInfo db_info;
        public static bool Load()
        {
            bool ret_net = JsonFile.Read<ServerNetInfo>("./gate.json", ref net_info);
            bool ret_db = JsonFile.Read<ServerDBInfo>("./db.json", ref db_info);
            return ret_net && ret_db;
        }
        /// <summary>
        /// 获取账号id对应的DB
        /// </summary>
        public static ushort GetDBByAccountIdx(long account_idx, eDBType type)
        {
            foreach (var obj in db_info.db_list)
            {
                if (obj.type != (ushort)type) continue;

                if (obj.account_begin == 0 && obj.account_end == 0)
                    return obj.id;
                else if (account_idx >= obj.account_begin && account_idx <= obj.account_end)
                    return obj.id;
            }
            Log.Warning("未找到账号对应的DB:" + type + " account_idx:" + account_idx);
            return 0;
        }
    }

    public class ServerNetInfo
    {
        public int log_level;
        public ushort server_uid;       //本区服务器唯一id
        public string ws_ip;
        public ushort ws_port;
        public string ip_for_client;
        public ushort port_for_client;
        public string ip_for_server;
        public ushort port_for_server;
        public byte speed_hack_check;   //加速检测
        public ushort max_connected;    //最大连接数
    }
    public class ServerDBInfo
    {
        public List<DBItems> db_list = new List<DBItems>();//数据库列表
    }
}
