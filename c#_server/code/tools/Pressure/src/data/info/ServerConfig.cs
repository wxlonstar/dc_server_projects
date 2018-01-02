using System;
using System.Collections.Generic;

namespace dc
{
    public class ServerConfig
    {
        public static ServerNetInfo net_info;
        public static bool Read()
        {
            if (JsonFile.Read<ServerNetInfo>("./pressure.json", ref net_info))
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
        public ushort net_client_count;
        public ushort net_send_count;
        public ushort net_send_size;
        //登陆
        public string login_server_ip;
        public ushort login_server_port;
        public ushort login_client_count;
        public ushort login_dis_time;//断开时间间隔
        //移动
        public string move_server_ip;
        public ushort move_server_port;
        public ushort move_client_count;
        public int move_start_account;//开始账号id
        public ushort move_time;//移动时间间隔
        //数据库
        public string db_server_ip;
        public ushort db_server_port;
        public ushort db_client_count;
        public int db_start_account;//开始账号id
        public ushort db_dis_time;//断开时间间隔
    }
}
