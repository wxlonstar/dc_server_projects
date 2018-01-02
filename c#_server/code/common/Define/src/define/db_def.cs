using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// db配置表结构
    /// </summary>
    public class DBItems
    {
        public ushort id;
        public ushort type;         //类型:对应eDBType
        public string name;         //dc_game数据库名
        public string address;      //dc_game数据库地址
        public ushort port;
        public string username;
        public string password;
        public long account_begin;  //开始账号：用于处理分库
        public long account_end;
    }
    /// <summary>
    /// 所在db：账号数据库默认为0，不可更改
    /// </summary>
    public struct DBID
    {
        public ushort game_id;      //逻辑数据库id
        public ushort center_id;    //世界数据库id
        public ushort log_id;       //日志数据库id

        public DBID(ushort g_id = 0, ushort c_id = 0, ushort l_id = 0)
        {
            game_id = g_id; center_id = c_id; log_id = l_id;
        }
        public void Reset()
        {
            game_id = center_id = log_id = 0;
        }
    }
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum eDBType
    {
        None,
        Member, //账号数据库只能存在一个：原因是gate需要根据账号id分配db，但是连接db之前，并不知道账号id；所以gate会随机连一个账号服务器来做账号登陆
        Game,
        Center,
        Log,

        Max
    }
}
