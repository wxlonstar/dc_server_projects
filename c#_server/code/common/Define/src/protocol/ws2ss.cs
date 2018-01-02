using System;
using System.Collections.Generic;

namespace dc.ws2ss
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_WS2SS;
        public const ushort CLIENT_ONLINE       = Begin + 3;        //检测是否在线

        public static void RegisterPools()
        {
            PacketPools.Register(CLIENT_ONLINE, "ws2ss.ClientOnline");
        }
    }

    /// <summary>
    /// 是否在线检测
    /// </summary>
    public class ClientOnline : PackBaseS2S
    {
        public long char_idx;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
        }
    }
}