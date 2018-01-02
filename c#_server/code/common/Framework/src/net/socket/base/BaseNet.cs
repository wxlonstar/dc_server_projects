using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// socket基类
    /// @author hannibal
    /// @time 2016-5-30
    /// </summary>
    public class BaseNet
    {
        public delegate void OnAcceptFunction(long conn_idx);
        public delegate void OnConnectedFunction(long conn_idx);
        public delegate void OnReceiveFunction(long conn_idx, ushort header,  ByteArray data);
        public delegate void OnCloseFunction(long conn_idx);

        public BaseNet.OnReceiveFunction OnReceive;
        public BaseNet.OnCloseFunction OnClose;

        public virtual void Setup()
        {

        }
        /// <summary>
        /// 外部调用，销毁socket
        /// </summary>
        public virtual void Destroy()
        {
            Close();
        }

        public virtual void Update()
        {

        }

        private static long send_count = 0;
        public virtual int Send(long conn_idx, ByteArray by)
        {
            ++send_count;
            if (send_count % 100000 == 0)
                Log.Debug("已发包:" + send_count);
            return 0;
        }
        /// <summary>
        /// 内部调用或底层触发
        /// </summary>
        public virtual void Close()
        {
            OnReceive = null;
            OnClose = null;
        }
        /// <summary>
        /// 连接是否有效
        /// </summary>
        public virtual bool Valid
        {
            get { return false; }
        }
    }
}
