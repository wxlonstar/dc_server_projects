using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 战斗服消息处理
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class FightMsgProc : ConnAppProc
    {
        public FightMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_FS2WS;
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
        }
        protected override void RegisterHandle()
        {
        }
        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }
    }
}
