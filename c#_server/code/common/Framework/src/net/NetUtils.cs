using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class NetUtils
    {
        private static ByteArray tmpSendBy = new ByteArray(1024, NetID.SendPacketMaxSize);
        public static ByteArray AllocSendPacket()
        {
            tmpSendBy.Clear();
            tmpSendBy.WriteUShort(0);//协议头，包长度
            return tmpSendBy;
        }

    }
}
