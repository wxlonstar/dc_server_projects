using System;
using System.Collections.Generic;

namespace dc.fs2gs
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_FS2GS;
        public const ushort PROXY_WS_MSG            = Begin + 1;

        public static void RegisterPools()
        {
            PacketPools.Register(PROXY_WS_MSG, "ProxyS2CMsg");
        }
    }
}