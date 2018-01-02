using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 服务器事件定义
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
        /// 显示弹出框信息
        /// </summary>
        public const string SHOW_MESSAGE = "SHOW_MESSAGE";
        /// <summary>
        /// 显示状态栏信息
        /// </summary>
        public const string SHOW_STATUS = "SHOW_STATUS";

        /// <summary>
        /// 收到服务器数据
        /// </summary>
        public const string SERVER_DATA = "SERVER_DATA";
        /// <summary>
        /// 收到ping数据
        /// </summary>
        public const string SERVER_PING = "SERVER_PING";


        /// <summary>
        /// 打开界面
        /// </summary>
        public const string OPEN_FORM = "OPEN_FORM";
    }
}
