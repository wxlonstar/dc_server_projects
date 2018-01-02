using System;
using System.Collections.Generic;

namespace dc.gs2fs
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_GS2FS;
        public const ushort PROXY_CLIENT_MSG        = Begin + 1;

        public static void RegisterPools()
        {
            PacketPools.Register(PROXY_CLIENT_MSG, "ProxyC2SMsg");
        }
    }
}
