using System;
using System.Collections.Generic;

namespace dc.c2ss
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_C2SS;
        
        public const ushort PING_NET                = Begin;            //ping网络
        public const ushort RESOURCE_LOADED         = Begin +1;         //前端必须资源加载完毕
        public const ushort ENTER_SCENE             = Begin + 2;        //传送

        public const ushort UNIT_MOVE               = Begin + 10;       //移动
        public const ushort UNIT_MODIFY_INT         = Begin + 11;       //属性改变
        public const ushort UNIT_MODIFY_STRING      = Begin + 12;       //属性改变

        public const ushort MAIL_LIST               = Begin + 20;       //邮件列表
        public const ushort MAIL_READ               = Begin + 21;       //读取列表
        public const ushort MAIL_WRITE              = Begin + 22;       //发送列表
        public const ushort MAIL_TAKE               = Begin + 23;       //提取列表
        public const ushort MAIL_DELETE             = Begin + 24;       //删除列表

        public const ushort RELATION_ADD            = Begin + 30;       //添加关系
        public const ushort RELATION_REMOVE         = Begin + 31;       //删除关系
        public const ushort RELATION_LIST           = Begin + 32;       //关系成员列表
        public const ushort RELATION_GIVE           = Begin + 33;       //赠送
        public const ushort RELATION_APPLY_CMD      = Begin + 34;       //请求操作

        public const ushort CHAT_SEND               = Begin + 50;       //聊天
        
        public static void RegisterPools()
        {
            PacketPools.Register(PING_NET, "c2ss.PingNet");
            PacketPools.Register(RESOURCE_LOADED, "c2ss.ResourceLoaded");
            PacketPools.Register(ENTER_SCENE, "c2ss.EnterScene");
            PacketPools.Register(UNIT_MOVE, "c2ss.UnitMove");
            PacketPools.Register(UNIT_MODIFY_INT, "c2ss.NotifyUpdatePlayerAttribInteger");
            PacketPools.Register(UNIT_MODIFY_STRING, "c2ss.NotifyUpdatePlayerAttribString");

            PacketPools.Register(MAIL_LIST, "c2ss.MailList");
            PacketPools.Register(MAIL_READ, "c2ss.MailRead");
            PacketPools.Register(MAIL_WRITE, "c2ss.MailWrite");
            PacketPools.Register(MAIL_TAKE, "c2ss.MailTake");
            PacketPools.Register(MAIL_DELETE, "c2ss.MailDelete");

            PacketPools.Register(RELATION_ADD, "c2ss.RelationAdd");
            PacketPools.Register(RELATION_REMOVE, "c2ss.RelationRemove");
            PacketPools.Register(RELATION_LIST, "c2ss.RelationList");
            PacketPools.Register(RELATION_GIVE, "c2ss.RelationGive");
            PacketPools.Register(RELATION_APPLY_CMD, "c2ss.RelationApplyCmd");

            PacketPools.Register(CHAT_SEND, "c2ss.ChatSend");
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
    /// <summary>
    /// 必须资源加载完毕
    /// </summary>
    public class SceneResLoaded : PackBaseC2S
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
    /// 场景传送
    /// </summary>
    public class EnterScene : PackBaseC2S
    {
        public uint scene_type;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            scene_type = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUInt(scene_type);
        }
    }
    #region AOI
    /// <summary>
    /// 移动
    /// </summary>
    public class UnitMove : PackBaseC2S
    {
        public Position2D pos = new Position2D();
        public eDirection dir = eDirection.NONE;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            pos.Read(by);
            dir = (eDirection)by.ReadByte();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            pos.Write(by);
            by.WriteByte((byte)dir);
        }
    }
    /// <summary>
    /// 属性改变
    /// </summary>
    public class NotifyUpdatePlayerAttribInteger : PackBaseC2S
    {
        public eUnitModType type;
        public long value;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            type = (eUnitModType)by.ReadUShort();
            value = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort((ushort)type);
            by.WriteLong(value);
        }
    }
    /// <summary>
    /// 属性改变
    /// </summary>
    public class NotifyUpdatePlayerAttribString : PackBaseC2S
    {
        public UnitID unit_idx = new UnitID();
        public eUnitModType type;
        public string value;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            type = (eUnitModType)by.ReadUShort();
            value = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort((ushort)type);
            by.WriteString(value);
        }
    }
    #endregion
    #region 邮件
    /// <summary>
    /// 邮件列表
    /// </summary>
    public class MailList : PackBaseC2S
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
    /// 读邮件
    /// </summary>
    public class MailRead : PackBaseC2S
    {
        public long mail_idx;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            mail_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(mail_idx);
        }
    }
    /// <summary>
    /// 发邮件
    /// </summary>
    public class MailWrite : PackBaseC2S
    {
        public MailWriteInfo info;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            info.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            info.Write(by);
        }
    }
    /// <summary>
    /// 读邮件
    /// </summary>
    public class MailTake : PackBaseC2S
    {
        public long mail_idx;
        public bool delete_mail;// 是否收完就删除
        public override void Read(ByteArray by)
        {
            base.Read(by);
            mail_idx = by.ReadLong();
            delete_mail = by.ReadBool();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(mail_idx);
            by.WriteBool(delete_mail);
        }
    }
    /// <summary>
    /// 删除邮件
    /// </summary>
    public class MailDelete : PackBaseC2S
    {
        public List<long> mail_list = new List<long>();
        public override void Init()
        {
            mail_list.Clear();
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            ushort count = by.ReadUShort();
            for (int i = 0; i < count; ++i)
            {
                mail_list.Add(by.ReadLong());
            }
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            ushort count = (ushort)mail_list.Count;
            by.WriteUShort(count);
            for (int i = 0; i < count; ++i)
            {
                by.WriteLong(mail_list[i]);
            }
        }
    } 
    #endregion
    #region 关系
    /// <summary>
    /// 添加关系
    /// </summary>
    public class RelationAdd : PackBaseC2S
    {
        public RelationAddTarget target_id;//目标
        public string message;
        public eRelationFlag flag;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            target_id.Read(by);
            message = by.ReadString();
            flag = (eRelationFlag)(by.ReadByte());
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            target_id.Write(by);
            by.WriteString(message);
            by.WriteByte((byte)flag);
        }
    }
    /// <summary>
    /// 移除关系
    /// </summary>
    public class RelationRemove : PackBaseC2S
    {
        public long target_char_idx;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            target_char_idx = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(target_char_idx);
        }
    }
    /// <summary>
    /// 关系列表
    /// </summary>
    public class RelationList : PackBaseC2S
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
    /// 好友赠送
    /// </summary>
    public class RelationGive : PackBaseC2S
    {
        public long target_char_idx;
        public ItemID item_id = new ItemID(); 

        public override void Read(ByteArray by)
        {
            base.Read(by);
            target_char_idx = by.ReadLong();
            item_id.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(target_char_idx);
            item_id.Write(by);
        }
    }
    /// <summary>
    /// 好友请求操作
    /// </summary>
    public class RelationApplyCmd : PackBaseC2S
    {
        public long event_idx;
        public long target_char_idx;
        public eRelationApplyCmd cmd;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            event_idx = by.ReadLong();
            target_char_idx = by.ReadLong();
            cmd = (eRelationApplyCmd)(by.ReadByte());
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(event_idx);
            by.WriteLong(target_char_idx);
            by.WriteByte((byte)cmd);
        }
    }
    #endregion
    #region 聊天
    /// <summary>
    /// 聊天
    /// </summary>
    public class ChatSend : PackBaseC2S
    {
        public eChatType type;
        public CharacterIdxOrName receiver;
        public string chat_content;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            type = (eChatType)(by.ReadByte());
            receiver.Read(by);
            chat_content = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte((byte)type);
            receiver.Write(by);
            by.WriteString(chat_content);
        }
    }
    #endregion
}
