using System;
using System.Collections.Generic;

namespace dc
{
    public class ServerConfig
    {
        public static ServerNetInfo info;
        public static bool Load()
        {
            if (JsonFile.Read<ServerNetInfo>("./bin_modify.json", ref info))
                return true;
            else 
                return false;
        }
    }

    public class ServerNetInfo
    {
        public class DBItems
        {
            public ushort id;
            public string name;     //dc_game数据库名
            public string address;  //dc_game数据库地址
            public ushort port;
            public string username;
            public string password;
        }
        public class FieldItems
        {
            public string name;
            public string fields;
            public int fixed_length;
        }
        public List<DBItems> db_list = new List<DBItems>();//数据库列表
        public List<FieldItems> field_list = new List<FieldItems>();//配置字段
    }
}
