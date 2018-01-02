using System;
using System.Collections.Generic;

namespace dc.c2gs
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_C2GS;

        public const ushort PING_NET                = Begin;            //ping网络

        public const ushort ENCRYPT                 = Begin + 1;        //加密
        public const ushort CLIENT_LOGIN            = Begin + 2;        //登陆

        public const ushort ENUM_CHAR               = Begin + 4;        //角色列表
        public const ushort CREATE_CHARACTER        = Begin + 5;        //创建角色
        public const ushort ENTER_GAME              = Begin + 6;
                
        public const ushort ROBOT_TEST              = Begin + 8;        //压力测试
        public const ushort SPEED_CHECK             = Begin + 9;        //加速检测


        public static void RegisterPools()
        {
            PacketPools.Register(PING_NET, "c2gs.PingNet");
            PacketPools.Register(ENCRYPT, "c2gs.EncryptInfo");
            PacketPools.Register(CLIENT_LOGIN, "c2gs.ClientLogin");
            PacketPools.Register(ENUM_CHAR, "c2gs.EnumCharacter");
            PacketPools.Register(CREATE_CHARACTER, "c2gs.CreateCharacter");
            PacketPools.Register(ENTER_GAME, "c2gs.EnterGame");
            PacketPools.Register(ROBOT_TEST, "c2gs.RobotTest");
            PacketPools.Register(SPEED_CHECK, "c2gs.SpeedCheck");
        }
    }
    /// <summary>
    /// ping网络状态
    /// </summary>
    public class PingNet : PackBaseC2S
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
    public class EncryptInfo : PacketBase
    {
        public uint version;// 客户端版本
        public uint flag;// 特殊标记
        public override void Read(ByteArray by)
        {
            base.Read(by);
            version = by.ReadUInt();
            flag = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUInt(version);
            by.WriteUInt(flag);
        }
    }

    /// <summary>
    /// 请求登陆
    /// </summary>
    public class ClientLogin : PackBaseC2S
    {
        public string name;
        public string psw;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            name = by.ReadString();
            psw = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteString(name);
            by.WriteString(psw);
        }
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    public class EnumCharacter : PackBaseC2S
    {
        public override void Read(ByteArray by)
        {
            base.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
        }
    }
    /// <summary>
    /// 创号
    /// </summary>
    public class CreateCharacter : PackBaseC2S
    {
        public string name;
        public uint flags;//标记组合
        public override void Read(ByteArray by)
        {
            base.Read(by);
            name = by.ReadString();
            flags = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteString(name);
            by.WriteUInt(flags);
        }
    }
    /// <summary>
    /// 进入游戏
    /// </summary>
    public class EnterGame : PackBaseC2S
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
    public class RobotTest : PackBaseC2S
	{
        public uint flags = 0;
        public int length = 500;
        public byte[] data = new byte[500];
        public override void Read(ByteArray by)
        {
            base.Read(by);
            flags = by.ReadUInt();
            length = by.ReadInt();
            by.Read(ref data, length, 0);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUInt(flags);
            by.WriteInt(length);
            by.WriteBytes(data, length);
        }
	}
    /// <summary>
    /// 加速检测
    /// </summary>
    public class SpeedCheck : PackBaseC2S
    {
        public int check_sn;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            check_sn = by.ReadInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteInt(check_sn);
        }
    }
}
