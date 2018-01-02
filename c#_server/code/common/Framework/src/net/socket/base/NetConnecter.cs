using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 客户端网络基类
    /// @author hannibal
    /// @time 2016-5-30
    /// </summary>
    public class NetConnecter : BaseNet
    {
        protected BaseNet.OnConnectedFunction OnConnected = null;

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
            OnConnected = null;
        }
    }
}
