using System;
using System.Collections.Generic;

namespace dc.gs2ws
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_GS2WS;
        public const ushort PROXY_CLIENT_MSG        = Begin + 1;
        public const ushort ROBOT_TEST              = Begin + 2;            //压力测试

        public const ushort CLIENT_LOGIN            = Begin + 3;
        public const ushort CREATE_CHARACTER        = Begin + 4;            //创建角色
        public const ushort ONLINE_COUNT            = Begin + 5;            //在线数

        public static void RegisterPools()
        {
            PacketPools.Register(PROXY_CLIENT_MSG, "ProxyC2SMsg");
            PacketPools.Register(ROBOT_TEST, "gs2ws.RobotTest");
            PacketPools.Register(CLIENT_LOGIN, "gs2ws.ClientLogin");
            PacketPools.Register(CREATE_CHARACTER, "gs2ws.CreateCharacter");
            PacketPools.Register(ONLINE_COUNT, "gs2ws.OnlineCount");
        }
    }
    /// <summary>
    /// 请求登陆
    /// </summary>
    public class ClientLogin : PackBaseS2S
    {
        public ClientUID client_uid;
        public string name;
        public string psw;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            name = by.ReadString();
            psw = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteString(name);
            by.WriteString(psw);
        }
    }
    /// <summary>
    /// 压力测试协议
    /// </summary>
    public class RobotTest : PackBaseS2S
    {
        public ClientUID client_uid;
        public int length = 500;
        public byte[] data = new byte[500];
        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            length = by.ReadInt();
            by.Read(ref data, length, 0);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteInt(length);
            by.WriteBytes(data, length);
        }
    }
    /// <summary>
    /// 请求创建角色
    /// </summary>
    public class CreateCharacter : PackBaseS2S
    {
        public ClientUID client_uid;
        public long account_idx;
        public ushort spid;
        public ushort db_id;
        public string name;
        public uint flags;//标记组合

        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            account_idx = by.ReadLong();
            spid = by.ReadUShort();
            db_id = by.ReadUShort();
            name = by.ReadString();
            flags = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteLong(account_idx);
            by.WriteUShort(spid);
            by.WriteUShort(db_id);
            by.WriteString(name);
            by.WriteUInt(flags);
        }
    }
    /// <summary>
    /// 在线数量
    /// </summary>
    public class OnlineCount : PackBaseS2S
    {
        public ushort count;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            count = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort(count);
        }
    }
}
