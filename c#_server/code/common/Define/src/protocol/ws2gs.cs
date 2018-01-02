using System;
using System.Collections.Generic;

namespace dc.ws2gs
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_WS2GS;
        public const ushort PROXY_WS_MSG        = Begin + 1;
        public const ushort ROBOT_TEST          = Begin + 2;            //压力测试

        public const ushort CLIENT_LOGIN        = Begin + 3;            //登陆结果
        public const ushort REQ_KICK_ACCOUNT    = Begin + 4;            //踢号
        public const ushort CREATE_CHARACTER    = Begin + 5;            //创建角色

        public static void RegisterPools()
        {
            PacketPools.Register(PROXY_WS_MSG, "ProxyS2CMsg");
            PacketPools.Register(ROBOT_TEST, "ws2gs.RobotTest");
            PacketPools.Register(CLIENT_LOGIN, "ws2gs.ClientLogin");
            PacketPools.Register(REQ_KICK_ACCOUNT, "ws2gs.ReqKickoutAccount");
            PacketPools.Register(CREATE_CHARACTER, "ws2gs.CreateCharacter");
        }
    }

    /// <summary>
    /// 登陆
    /// </summary>
    public class ClientLogin : PackBaseS2S
    {
        public ClientUID client_uid;
        // 登录结果
        public eLoginResult login_result;
        public long account_idx = 0;
        public ushort spid = 0;
        public ushort ss_uid = 0;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            login_result = (eLoginResult)by.ReadByte();
            account_idx = by.ReadLong();
            spid = by.ReadUShort();
            ss_uid = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteByte((byte)login_result);
            by.WriteLong(account_idx);
            by.WriteUShort(spid);
            by.WriteUShort(ss_uid);
        }
    }
    /// <summary>
    /// ws请求踢出帐号：踢号只能从gs开始
    /// </summary>
    public class ReqKickoutAccount : PackBaseS2S
    {
        public long account_idx;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            account_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(account_idx);
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
        public eCreateCharResult result;
        public long account_idx;
        public long char_idx;// 创建成功的角色id
        public override void Read(ByteArray by)
        {
            base.Read(by);
            result = (eCreateCharResult)by.ReadByte();
            account_idx = by.ReadLong();
            char_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte((byte)result);
            by.WriteLong(account_idx);
            by.WriteLong(char_idx);
        }
    }
}
