using System;
using System.Collections.Generic;

namespace dc
{
	/// <summary>
	/// 压力测试数据
	/// </summary>
    public class sPressureNetInfo
    {
        public string ip;
        public ushort port;
        public ushort client_count;
        public ushort send_count_per_second;
        public ushort send_size_per_packet;
    }
    /// <summary>
    /// 登陆测试数据
    /// </summary>
    public class sPressureLoginInfo
    {
        public string ip;
        public ushort port;
        public ushort client_count;
        public float dis_conn_time;
    }
    /// <summary>
    /// 移动测试数据
    /// </summary>
    public class sPressureMoveInfo
    {
        public string ip;
        public ushort port;
        public ushort client_count;
        public float move_time;
        public int start_account;
    }
    /// <summary>
    /// 数据库测试数据
    /// </summary>
    public class sPressureDBInfo
    {
        public string ip;
        public ushort port;
        public ushort client_count;
        public float dis_conn_time;
        public int start_account;
    }
}
