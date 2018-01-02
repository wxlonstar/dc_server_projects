using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace dc
{
    /// <summary>
    /// iocp单个对象缓冲
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// 连接id
        /// </summary>
        public long ConnId { get; set; }
        /// <summary>
        /// 远程地址
        /// </summary>
        public EndPoint Remote { get; set; }
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
            Remote = null;
            Socket = null;
        }
    }
}
