﻿using System;
using System.Collections.Generic;

namespace dc.fs2c
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_FS2C;
        public const ushort PING_NET                = Begin;            //ping网络
        
        public static void RegisterPools()
        {
            PacketPools.Register(PING_NET, "fs2c.PingNet");
        }
    }
    /// <summary>
    /// ping网络状态
    /// </summary>
    public class PingNet : PackBaseS2C
    {
        public uint packet_id = 0;  //发送包id
        public long tick = 0;       //发送时间，记录延迟
        public uint flags = 0;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            packet_id = by.ReadUInt();
            tick = by.ReadLong();
            flags = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUInt(packet_id);
            by.WriteLong(tick);
            by.WriteUInt(flags);
        }
    }
}