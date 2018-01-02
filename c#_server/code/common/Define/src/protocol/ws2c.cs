using System;
using System.Collections.Generic;

namespace dc.ws2c
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_WS2C;
        
        public const ushort PING_NET                = Begin;            //ping网络
        public const ushort CLIENT_EVENT            = Begin + 1;        //游戏事件
        public const ushort GAME_ERROR              = Begin + 2;        //游戏错误
        public const ushort SHUTDOWN_SERVER         = Begin + 3;        //关服
        public const ushort SERVER_TIME             = Begin + 4;        //服务器当前时间

        public static void RegisterPools()
        {
            PacketPools.Register(PING_NET, "ws2c.PingNet");
            PacketPools.Register(CLIENT_EVENT, "ws2c.ClientEvent");
            PacketPools.Register(GAME_ERROR, "ws2c.GameError");
            PacketPools.Register(SHUTDOWN_SERVER, "ws2c.ShutdownServer");
            PacketPools.Register(SERVER_TIME, "ws2c.ServerTime");
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
    /// <summary>
    /// 客户端事件
    /// </summary>
    public class ClientEvent : PackBaseS2C
    {
        public eClientEvent client_event;
        public eClientEventAction action;
        public string desc = "";

        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_event = (eClientEvent)by.ReadUShort();
            action = (eClientEventAction)by.ReadUShort();
            desc = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort((ushort)client_event);
            by.WriteUShort((ushort)action);
            by.WriteString(desc);
        }
    }
    /// <summary>
    /// 操作错误
    /// </summary>
    public class GameError : PackBaseS2C
    {
        public eErrorType type;
        public eErrorAction action;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            type = (eErrorType)by.ReadUShort();
            action = (eErrorAction)by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort((ushort)type);
            by.WriteUShort((ushort)action);
        }
    }
    /// <summary>
    /// 关服
    /// </summary>
    public class ShutdownServer : PackBaseS2C
    {
        public ushort leave_time;//剩余时间

        public override void Read(ByteArray by)
        {
            base.Read(by);
            leave_time = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort(leave_time);
        }
    }
    /// <summary>
    /// 服务器时间
    /// </summary>
    public class ServerTime : PackBaseS2C
    {
        public long server_time;//2009到现在经过的毫秒数

        public override void Read(ByteArray by)
        {
            base.Read(by);
            server_time = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(server_time);
        }
    }
}
