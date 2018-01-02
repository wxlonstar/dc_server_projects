using System;
using System.Collections.Generic;

namespace dc.ss2gl
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_SS2GL;
        public const ushort PING_NET                = Begin;            //ping网络
        public const ushort LOGIN_CLIENT            = Begin + 1;
        public const ushort LOGOUT_CLIENT           = Begin + 2;        //登出
        
        public const ushort UNIT_MODIFY_INT         = Begin + 10;       //属性改变
        public const ushort UNIT_MODIFY_STRING      = Begin + 11;       //属性改变
        
        public const ushort RELATION_ADD            = Begin + 30;       //添加关系
        public const ushort RELATION_REMOVE         = Begin + 31;       //删除关系
        public const ushort RELATION_GIVE           = Begin + 32;       //关系成员列表
        public const ushort RELATION_APPLY_CMD      = Begin + 33;       //请求操作

        public static void RegisterPools()
        {
            PacketPools.Register(PING_NET, "ss2gl.PingNet");

            PacketPools.Register(LOGIN_CLIENT, "ss2gl.LoginClient");
            PacketPools.Register(LOGOUT_CLIENT, "ss2gl.LogoutClient");
            PacketPools.Register(UNIT_MODIFY_INT, "ss2gl.NotifyUpdatePlayerAttribInteger");
            PacketPools.Register(UNIT_MODIFY_STRING, "ss2gl.NotifyUpdatePlayerAttribString");

            PacketPools.Register(RELATION_ADD, "ss2gl.RelationAdd");
            PacketPools.Register(RELATION_REMOVE, "ss2gl.RelationRemove");
            PacketPools.Register(RELATION_GIVE, "ss2gl.RelationGive");
            PacketPools.Register(RELATION_APPLY_CMD, "ss2gl.RelationApplyCmd");
        }
    }
    /// <summary>
    /// ping网络状态
    /// </summary>
    public class PingNet : PackBaseS2S
    {
        public ClientUID client_uid;
        public uint packet_id = 0;  //发送包id
        public long tick = 0;       //发送时间，记录延迟
        public uint flags = 0;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            packet_id = by.ReadUInt();
            tick = by.ReadLong();
            flags = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
            by.WriteUInt(packet_id);
            by.WriteLong(tick);
            by.WriteUInt(flags);
        }
    }
    #region 角色
    /// <summary>
    /// 角色信息
    /// </summary>
    public class LoginClient : PackBaseS2S
    {
        public PlayerInfoForGL data = new PlayerInfoForGL();
        public override void Read(ByteArray by)
        {
            base.Read(by);
            data.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            data.Write(by);
        }
    }
    /// <summary>
    /// 客户端登出
    /// </summary>
    public class LogoutClient : PackBaseS2S
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
    /// <summary>
    /// 属性改变
    /// </summary>
    public class NotifyUpdatePlayerAttribInteger : PackBaseS2S
    {
        public long char_idx;
        public eUnitModType type;
        public long value;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            type = (eUnitModType)by.ReadUShort();
            value = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            by.WriteUShort((ushort)type);
            by.WriteLong(value);
        }
    }
    /// <summary>
    /// 属性改变
    /// </summary>
    public class NotifyUpdatePlayerAttribString : PackBaseS2S
    {
        public long char_idx;
        public eUnitModType type;
        public string value;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            type = (eUnitModType)by.ReadUShort();
            value = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            by.WriteUShort((ushort)type);
            by.WriteString(value);
        }
    }
    #endregion
    #region 关系
    /// <summary>
    /// 添加关系
    /// </summary>
    public class RelationAdd : PackBaseS2S
    {
        public long char_idx;
        public RelationAddTarget target_id;//目标
        public eRelationFlag flag;
        public string message;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            target_id.Read(by);
            flag = (eRelationFlag)(by.ReadByte());
            message = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            target_id.Write(by);
            by.WriteByte((byte)flag);
            by.WriteString(message);
        }
    }
    /// <summary>
    /// 移除关系
    /// </summary>
    public class RelationRemove : PackBaseS2S
    {
        public long char_idx;
        public long target_char_idx;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            target_char_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            by.WriteLong(target_char_idx);
        }
    }
    /// <summary>
    /// 好友赠送
    /// </summary>
    public class RelationGive : PackBaseS2S
    {
        public long char_idx;
        public long target_char_idx;
        public ItemID item_id = new ItemID();

        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            target_char_idx = by.ReadLong();
            item_id.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            by.WriteLong(target_char_idx);
            item_id.Write(by);
        }
    }
    /// <summary>
    /// 好友请求操作
    /// </summary>
    public class RelationApplyCmd : PackBaseS2S
    {
        public long event_idx;
        public long char_idx;
        public long target_char_idx;
        public eRelationApplyCmd cmd;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            event_idx = by.ReadLong();
            char_idx = by.ReadLong();
            target_char_idx = by.ReadLong();
            cmd = (eRelationApplyCmd)(by.ReadByte());
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(event_idx);
            by.WriteLong(char_idx);
            by.WriteLong(target_char_idx);
            by.WriteByte((byte)cmd);
        }
    }
    #endregion
}
