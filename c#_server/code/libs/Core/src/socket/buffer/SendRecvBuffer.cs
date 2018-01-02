using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace dc
{
    /// <summary>
    /// 单个连接开辟的缓冲
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public class SendRecvBuffer
    {
        /// <summary>
        /// 连接id
        /// </summary>
        public long ConnId { get; set; }
        /// <summary>
        /// 通信SOKET
        /// </summary>
        public Socket Socket { get; set; }
        /// <summary>
        /// 传递给iocp的buffer
        /// </summary>
        public byte[] Buffer = new byte[SocketUtils.SendRecvMaxSize];

        public void Reset()
        {
            ConnId = 0;
            Socket = null;
        }
    }
}
