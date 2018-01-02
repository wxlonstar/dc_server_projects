using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class NetID
    {
        public const int InitByteArraySize = 32767;
        public const int MaxByteArraySize = 65535;

        public const int PacketHeadSize = 2;

        public const int SendPacketMaxSize = 4096;
        public const int RecvPacketMaxSize = 4096;
    }
    /// <summary>
    /// 网络事件
    /// </summary>
    public class NetEventID
    {
        public const string NET_DISCONNECT  = "NET_DISCONNECT";     //网络连接断开
        public const string CONNECT_SUCCEED = "CONNECT_SUCCEED";	//服务器连接成功
        public const string CONNECT_FAILED  = "CONNECT_FAILED";     //服务器连接失败
    }
    /// <summary>
    /// socket类型
    /// </summary>
    public enum eSocketType
    {
        NONE = 0,
        TCP_CLIENT,
        TCP_SERVER,
        WEB_CLIENT,
        WEB_SERVER,
    }
}
