using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 服务器网络基类
    /// @author hannibal
    /// @time 2016-5-30
    /// </summary>
    public abstract class NetAccepter : BaseNet
    {
        protected BaseNet.OnAcceptFunction OnAccept = null;

        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
        }

        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// 内部调用或底层触发
        /// </summary>
        public override void Close()
        {
            base.Close();
            OnAccept = null;
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="conn_idx"></param>
        public abstract void CloseConn(long conn_idx);
        /// <summary>
        /// 获取连接ip和端口
        /// </summary>
        /// <param name="conn_idx"></param>
        /// <returns></returns>
        public abstract string GetConnIP(long conn_idx);
        public abstract ushort GetConnPort(long conn_idx);
    }
}
