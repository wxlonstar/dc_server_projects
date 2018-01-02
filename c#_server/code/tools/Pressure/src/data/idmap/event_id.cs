using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 客户端事件定义
    /// @author hannibal
    /// @time 2016-8-18
    /// </summary>
    public class ClientEventID
    {
        /// <summary>
        /// 新连接打开
        /// </summary>
        public const string NET_CONNECTED_OPEN = "NET_CONNECTED_OPEN";
        /// <summary>
        /// 连接关闭
        /// </summary>
        public const string NET_CONNECTED_CLOSE = "NET_CONNECTED_CLOSE";
        /// <summary>
        /// 切换压力测试
        /// </summary>
        public const string SWITCH_PRESSURE = "SWITCH_PRESSURE";

        /// <summary>
        /// 收到数据
        /// </summary>
        public const string RECV_DATA = "RECV_DATA";
    }
}
