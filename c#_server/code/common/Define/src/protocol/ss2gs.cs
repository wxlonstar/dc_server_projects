using System;
using System.Collections.Generic;

namespace dc.ss2gs
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_SS2GS;
        public const ushort PROXY_SS_MSG            = Begin + 1;
        public const ushort ROBOT_TEST              = Begin + 2;            //压力测试

        public const ushort CREATE_CHARACTER        = Begin + 3;
        public const ushort REQ_KICK_ACCOUNT        = Begin + 4;            //踢号
        public const ushort ENTER_GAME              = Begin + 5;
        public const ushort NOTIFY_SERVER           = Begin + 6;            //分配给玩家的fs


        public static void RegisterPools()
        {
            PacketPools.Register(PROXY_SS_MSG, "ProxyS2CMsg");
            PacketPools.Register(ROBOT_TEST, "ss2gs.RobotTest");
            PacketPools.Register(CREATE_CHARACTER, "ss2gs.CreateCharacter");
            PacketPools.Register(REQ_KICK_ACCOUNT, "ss2gs.ReqKickoutAccount");
            PacketPools.Register(ENTER_GAME, "ss2gs.EnterGame");
            PacketPools.Register(NOTIFY_SERVER, "ss2gs.NotifyServer");
        }
    }
    /// <summary>
    /// 请求创建角色
    /// </summary>
    public class CreateCharacter : PackBaseS2S
    {
        public ClientUID client_uid;
        public eCreateCharResult result;
        public long char_idx;
        public string char_name;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            result = (eCreateCharResult)by.ReadByte();
            char_idx = by.ReadLong();
            char_name = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteByte((byte)result);
            by.WriteLong(char_idx);
            by.WriteString(char_name);
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
    /// 进入游戏:角色必须资源等已经加载完毕，可以进入场景
    /// </summary>
    public class EnterGame : PackBaseS2S
    {
        public ClientUID client_uid;
        public long char_idx;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            char_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteLong(char_idx);
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
    /// 分配给玩家的fs
    /// </summary>
    public class NotifyServer : PackBaseS2S
    {
        public long account_idx;
        public eServerType s_type;
        public ushort fs_uid;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            account_idx = by.ReadLong();
            s_type = (eServerType)by.ReadByte();
            fs_uid = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(account_idx);
            by.WriteByte((byte)s_type);
            by.WriteUShort(fs_uid);
        }
    }
}
