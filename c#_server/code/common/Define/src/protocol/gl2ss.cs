using System;
using System.Collections.Generic;

namespace dc.gl2ss
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_GL2SS;

        public const ushort PING_NET                = Begin;            //ping网络

        public const ushort RELATION_ADD            = Begin + 30;       //添加关系
        public const ushort RELATION_REMOVE         = Begin + 31;       //删除关系
        public const ushort RELATION_LIST           = Begin + 32;       //关系成员列表
        public const ushort RELATION_GIVE           = Begin + 33;       //赠送

        public static void RegisterPools()
        {
            PacketPools.Register(PING_NET, "gl2ss.PingNet");

            PacketPools.Register(RELATION_ADD, "gl2ss.RelationAdd");
            PacketPools.Register(RELATION_REMOVE, "gl2ss.RelationRemove");
            PacketPools.Register(RELATION_LIST, "gl2ss.RelationList");
            PacketPools.Register(RELATION_GIVE, "gl2ss.RelationGive");
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
    #region 关系
    /// <summary>
    /// 添加关系：发给对方
    /// </summary>
    public class RelationAdd : PackBaseS2S
    {
        public long event_idx;
        public long char_idx;
        public PlayerIDName player_id = new PlayerIDName();//请求者id
        public string message;
        public eRelationFlag flag;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            event_idx = by.ReadLong();
            char_idx = by.ReadLong();
            player_id.Read(by);
            flag = (eRelationFlag)(by.ReadByte());
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(event_idx);
            by.WriteLong(char_idx);
            player_id.Write(by);
            by.WriteByte((byte)flag);
        }
    }
    /// <summary>
    /// 移除关系：分别发给双方
    /// </summary>
    public class RelationRemove : PackBaseS2S
    {
        public long char_idx;
        public long target_idx;//需要移除的角色id

        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            target_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            by.WriteLong(target_idx);
        }
    }
    /// <summary>
    /// 关系列表：分别发给双方
    /// </summary>
    public class RelationList : PackBaseS2S
    {
        public long char_idx;
        public RelationInfo relation_info = new RelationInfo();

        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            relation_info.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            relation_info.Write(by);
        }
    }
    /// <summary>
    /// 好友赠送：发给对方
    /// </summary>
    public class RelationGive : PackBaseS2S
    {
        public long char_idx;
        public PlayerIDName src_player_id = new PlayerIDName();//赠送者id
        public ItemID item_id = new ItemID();

        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            src_player_id.Read(by);
            item_id.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            src_player_id.Write(by);
            item_id.Write(by);
        }
    }
    #endregion
}