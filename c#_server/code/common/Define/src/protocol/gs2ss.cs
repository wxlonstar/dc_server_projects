using System;
using System.Collections.Generic;

namespace dc.gs2ss
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_GS2SS;
        public const ushort PROXY_CLIENT_MSG        = Begin + 1;
        public const ushort ROBOT_TEST              = Begin + 2;        //压力测试

        public const ushort ENUM_CHAR               = Begin + 3;        //角色列表
        public const ushort ENTER_GAME              = Begin + 4;
        public const ushort LOGOUT_ACCOUNT          = Begin + 5;
        public const ushort KICK_ACCOUNT            = Begin + 6;


        public static void RegisterPools()
        {
            PacketPools.Register(PROXY_CLIENT_MSG, "ProxyC2SMsg");
            PacketPools.Register(ROBOT_TEST, "gs2ss.RobotTest");
            PacketPools.Register(ENUM_CHAR, "gs2ss.EnumCharacter");
            PacketPools.Register(ENTER_GAME, "gs2ss.EnterGame");
            PacketPools.Register(LOGOUT_ACCOUNT, "gs2ss.LogoutAccount");
            PacketPools.Register(KICK_ACCOUNT, "gs2ss.KickoutAccount");
        }
    }
    /// <summary>
    /// 角色列表
    /// </summary>
    public class EnumCharacter : PackBaseS2S
    {
        public ClientUID client_uid;
        public long account_idx;
        public ushort game_db_id;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            account_idx = by.ReadLong();
            game_db_id = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteLong(account_idx);
            by.WriteUShort(game_db_id);
        }
    }
    /// <summary>
    /// 进入游戏
    /// </summary>
    public class EnterGame : PackBaseS2S
    {
        public ClientUID client_uid;
        public long account_idx;
        public long char_idx;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            account_idx = by.ReadLong();
            char_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteLong(account_idx);
            by.WriteLong(char_idx);
        }
    }
    /// <summary>
    /// 帐号登出
    /// </summary>
    public class LogoutAccount : PackBaseS2S
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
    /// 踢出帐号
    /// </summary>
    public class KickoutAccount : PackBaseS2S
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
}
