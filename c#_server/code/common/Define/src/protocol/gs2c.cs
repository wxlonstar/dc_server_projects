using System;
using System.Collections.Generic;

namespace dc.gs2c
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_GS2C;
        
        public const ushort PING_NET                = Begin;            //ping网络

        public const ushort ENCRYPT                 = Begin + 1;        //加密
        public const ushort CLIENT_LOGIN            = Begin + 2;        //登陆结果

        public const ushort CREATE_CHARACTER        = Begin + 4;        //创建角色
        public const ushort ENTER_GAME              = Begin + 5;

        public const ushort ROBOT_TEST              = Begin + 8;        //压力测试
        public const ushort SPEED_CHECK             = Begin + 9;        //加速检测

        public const ushort CLIENT_EVENT            = Begin + 10;       //游戏事件
        public const ushort GAME_ERROR              = Begin + 11;       //游戏错误

        public static void RegisterPools()
        {
            PacketPools.Register(PING_NET, "gs2c.PingNet");
            PacketPools.Register(ENCRYPT, "gs2c.EncryptInfo");
            PacketPools.Register(CLIENT_LOGIN, "gs2c.ClientLogin");
            PacketPools.Register(CREATE_CHARACTER, "gs2c.CreateCharacter");
            PacketPools.Register(ENTER_GAME, "gs2c.EnterGame");
            PacketPools.Register(ROBOT_TEST, "gs2c.RobotTest");
            PacketPools.Register(SPEED_CHECK, "gs2c.SpeedCheck");
            PacketPools.Register(CLIENT_EVENT, "gs2c.ClientEvent");
            PacketPools.Register(GAME_ERROR, "gs2c.GameError");
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
    #region 登录
    /// <summary>
    /// 加密
    /// </summary>
    public class EncryptInfo : PackBaseS2C
    {
        public ushort key = 0;
        public uint flags = 0;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            key = by.ReadUShort();
            flags = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort(key);
            by.WriteUInt(flags);
        }
    }

    /// <summary>
    /// 登陆
    /// </summary>
    public class ClientLogin : PackBaseS2C
    {
        // 登录结果
        public eLoginResult login_result;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            login_result = (eLoginResult)by.ReadByte();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte((byte)login_result);
        }
    }

    /// <summary>
    /// 创号
    /// </summary>
    public class CreateCharacter : PackBaseS2C
    {
        public eCreateCharResult result;
        public long char_idx;// 创建成功的角色id
        public override void Read(ByteArray by)
        {
            base.Read(by);
            result = (eCreateCharResult)by.ReadByte();
            char_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte((byte)result);
            by.WriteLong(char_idx);
        }
    }
    /// <summary>
    /// 进入游戏:角色必须资源等已经加载完毕，可以进入场景
    /// </summary>
    public class EnterGame : PackBaseS2C
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
    #endregion
    /// <summary>
    /// 压力测试协议
    /// </summary>
    public class RobotTest : PackBaseS2C
    {
        public int length = 500;
        public byte[] data = new byte[500];
        public override void Read(ByteArray by)
        {
            base.Read(by);
            length = by.ReadInt();
            by.Read(ref data, length, 0);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteInt(length);
            by.WriteBytes(data, length);
        }
    }
    /// <summary>
    /// 加速检测
    /// </summary>
    public class SpeedCheck : PackBaseS2C
    {
        public int delay_time;
        public int check_sn;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            delay_time = by.ReadInt();
            check_sn = by.ReadInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteInt(delay_time);
            by.WriteInt(check_sn);
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
}
