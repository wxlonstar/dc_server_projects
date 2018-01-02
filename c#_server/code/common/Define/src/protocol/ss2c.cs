using System;
using System.Collections.Generic;

namespace dc.ss2c
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_SS2C;
        
        public const ushort PING_NET                = Begin;            //ping网络
        public const ushort CLIENT_EVENT            = Begin + 1;        //游戏事件
        public const ushort GAME_ERROR              = Begin + 2;        //游戏错误
        public const ushort ENUM_CHAR               = Begin + 3;        //角色列表
        public const ushort CHARACTER_INFO          = Begin + 5;        //角色信息
        public const ushort ENTER_SCENE             = Begin + 6;        //进入场景

        public const ushort UNIT_MOVE               = Begin + 10;       //移动
        public const ushort ENTER_AOI               = Begin + 11;       //进入aoi
        public const ushort LEAVE_AOI               = Begin + 12;       //离开
        public const ushort UNIT_MODIFY_INT         = Begin + 13;       //属性改变
        public const ushort UNIT_MODIFY_STRING      = Begin + 14;       //属性改变

        public const ushort MAIL_COUNT              = Begin + 20;       //获取普通邮件未读和已读数量
        public const ushort MAIL_LIST               = Begin + 21;       //获取邮件列表
        public const ushort MAIL_READ               = Begin + 22;       //读取邮件
        public const ushort MAIL_COMMAND            = Begin + 23;       //邮件操作结果

        public const ushort RELATION_ADD            = Begin + 30;       //添加关系
        public const ushort RELATION_REMOVE         = Begin + 31;       //删除关系
        public const ushort RELATION_LIST           = Begin + 32;       //关系成员列表
        public const ushort RELATION_GIVE           = Begin + 33;       //赠送

        public const ushort COUNTER_LIST            = Begin + 40;       //次数列表
        public const ushort COUNTER_MODIFY          = Begin + 41;       //次数改变

        public const ushort CHAT_RECV               = Begin + 50;       //聊天
        public const ushort CHAT_RESULT             = Begin + 51;       //聊天结果

        public static void RegisterPools()
        {
            PacketPools.Register(PING_NET, "ss2c.PingNet");
            PacketPools.Register(CLIENT_EVENT, "ss2c.ClientEvent");
            PacketPools.Register(GAME_ERROR, "ss2c.GameError");
            PacketPools.Register(ENUM_CHAR, "ss2c.EnumCharacter");
            PacketPools.Register(CHARACTER_INFO, "ss2c.CharacterInfo");
            PacketPools.Register(ENTER_SCENE, "ss2c.EnterScene");
            PacketPools.Register(UNIT_MOVE, "ss2c.UnitMove");
            PacketPools.Register(ENTER_AOI, "ss2c.UnitEnterAOI");
            PacketPools.Register(LEAVE_AOI, "ss2c.UnitLeaveAOI");
            PacketPools.Register(UNIT_MODIFY_INT, "ss2c.NotifyUpdatePlayerAttribInteger");
            PacketPools.Register(UNIT_MODIFY_STRING, "ss2c.NotifyUpdatePlayerAttribString");

            PacketPools.Register(MAIL_COUNT, "ss2c.MailCount");
            PacketPools.Register(MAIL_LIST, "ss2c.MailList");
            PacketPools.Register(MAIL_READ, "ss2c.MailRead");
            PacketPools.Register(MAIL_COMMAND, "ss2c.MailCommand");

            PacketPools.Register(RELATION_ADD, "ss2c.RelationAdd");
            PacketPools.Register(RELATION_REMOVE, "ss2c.RelationRemove");
            PacketPools.Register(RELATION_LIST, "ss2c.RelationList");
            PacketPools.Register(RELATION_GIVE, "ss2c.RelationGive");

            PacketPools.Register(COUNTER_LIST, "ss2c.CounterList");
            PacketPools.Register(COUNTER_MODIFY, "ss2c.CounterModify");

            PacketPools.Register(CHAT_RECV, "ss2c.ChatRecv");
            PacketPools.Register(CHAT_RESULT, "ss2c.ChatResult");
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
    /// 角色列表
    /// </summary>
    public class EnumCharacter : PackBaseS2C
    {
        // 上次选择角色序号
        public byte last_sel_idx;
        public List<CharacterLogin> list = new List<CharacterLogin>();
        public override void Init()
        {
            base.Init();
            list.Clear();
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            last_sel_idx = by.ReadByte();
            int count = by.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                CharacterLogin obj = new CharacterLogin();
                obj.Read(by);
                list.Add(obj);
            }
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte(last_sel_idx);
            by.WriteInt(list.Count);
            for (int i = 0; i < list.Count; ++i)
            {
                CharacterLogin obj = list[i];
                obj.Write(by);
            }
        }
    }
    /// <summary>
    /// 场景切换
    /// </summary>
    public class EnterScene : PackBaseS2C
    {
        public uint scene_type;
        public long scene_instance_id;
        public Position2D pos = new Position2D();
        public eDirection dir;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            scene_type = by.ReadUInt();
            scene_instance_id = by.ReadLong();
            pos.Read(by);
            dir = (eDirection)by.ReadByte();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUInt(scene_type);
            by.WriteLong(scene_instance_id);
            pos.Write(by);
            by.WriteByte((byte)dir);
        }
    }

    /// <summary>
    /// 角色信息
    /// </summary>
    public class CharacterInfo : PackBaseS2C
    {
        public PlayerInfoForClient data = new PlayerInfoForClient();

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
    #region AOI
    /// <summary>
    /// 单位进入
    /// </summary>
    public class UnitEnterAOI : PackBaseS2C
    {
        public UnitID unit_idx = new UnitID();
        public Position2D pos = new Position2D();
        public eDirection dir;
        public uint flags = 0;//进入标记
        public UnitAOIInfo unit_info = null;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            unit_idx.Read(by);
            pos.Read(by);
            dir = (eDirection)by.ReadByte();
            flags = by.ReadUInt();
            unit_info = unit.GetUnitInfo(unit_idx.type);
            if (unit_info != null) unit_info.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            unit_idx.Write(by);
            pos.Write(by);
            by.WriteByte((byte)dir);
            by.WriteUInt(flags);
            unit_info.Write(by);
        }
    }
    /// <summary>
    /// 单位离开
    /// </summary>
    public class UnitLeaveAOI : PackBaseS2C
    {
        public UnitID unit_idx = new UnitID();
        public uint flags = 0;//离开标记
        public override void Init()
        {
            base.Init();
            flags = 0;
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            unit_idx.Read(by);
            flags = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            unit_idx.Write(by);
            by.WriteUInt(flags);
        }
    }
    /// <summary>
    /// 单位移动
    /// </summary>
    public class UnitMove : PackBaseS2C
    {
        public UnitID unit_idx = new UnitID();
        public Position2D pos = new Position2D();
        public eDirection dir = eDirection.NONE;
        public uint flags = 0;//移动标记
        public override void Init()
        {
            base.Init();
            flags = 0;
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            unit_idx.Read(by);
            pos.Read(by);
            dir = (eDirection)by.ReadByte();
            flags = by.ReadUInt();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            unit_idx.Write(by);
            pos.Write(by);
            by.WriteByte((byte)dir);
            by.WriteUInt(flags);
        }
    }
    /// <summary>
    /// 属性改变
    /// </summary>
    public class NotifyUpdatePlayerAttribInteger : PackBaseS2C
    {
        public UnitID unit_idx = new UnitID();
        public eUnitAttrAction action = eUnitAttrAction.Unknow;
        public eUnitModType type;
        public long value;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            unit_idx.Read(by);
            action = (eUnitAttrAction)by.ReadUShort();
            type = (eUnitModType)by.ReadUShort();
            value = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            unit_idx.Write(by);
            by.WriteUShort((ushort)action);
            by.WriteUShort((ushort)type);
            by.WriteLong(value);
        }
    }
    /// <summary>
    /// 属性改变
    /// </summary>
    public class NotifyUpdatePlayerAttribString : PackBaseS2C
    {
        public UnitID unit_idx = new UnitID();
        public eUnitAttrAction action = eUnitAttrAction.Unknow;
        public eUnitModType type;
        public string value;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            unit_idx.Read(by);
            action = (eUnitAttrAction)by.ReadUShort();
            type = (eUnitModType)by.ReadUShort();
            value = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            unit_idx.Write(by);
            by.WriteUShort((ushort)action);
            by.WriteUShort((ushort)type);
            by.WriteString(value);
        }
    }
    #endregion

    #region 邮件
    /// <summary>
    /// 邮件数量
    /// </summary>
    public class MailCount : PackBaseS2C
    {	
        // 总计邮件数量
        public ushort	total_mail_count;
        // 未读邮件数量
        public ushort unread_mail_count;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            total_mail_count = by.ReadUShort();
            unread_mail_count = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort(total_mail_count);
            by.WriteUShort(unread_mail_count);
        }
    }
    /// <summary>
    /// 邮件列表
    /// </summary>
    public class MailList : PackBaseS2C
    {
        // 主题信息
        public List<MailTitleInfo> mail_list = new List<MailTitleInfo>();
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
                MailTitleInfo info = new MailTitleInfo();
                info.Read(by);
                mail_list.Add(info);
            }
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            ushort count = (ushort)mail_list.Count;
            by.WriteUShort(count);
            for (int i = 0; i < count; ++i)
            {
                mail_list[i].Write(by);
            }
        }
    }
    /// <summary>
    /// 读邮件
    /// </summary>
    public class MailRead : PackBaseS2C
    {
        // 详细信息
        public MailInfo mail_info = new MailInfo();
        public override void Read(ByteArray by)
        {
            base.Read(by);
            mail_info.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            mail_info.Write(by);
        }
    }
    /// <summary>
    /// 邮件操作
    /// </summary>
    public class MailCommand : PackBaseS2C
    {
        public long mail_idx;
        public eMailCommandType command_type;
        public eMailCommandError error_type;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            mail_idx = by.ReadLong();
            command_type = (eMailCommandType)by.ReadByte();
            error_type = (eMailCommandError)by.ReadByte();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(mail_idx);
            by.WriteByte((byte)command_type);
            by.WriteByte((byte)error_type);
        }
    }
    #endregion

    #region 关系
    /// <summary>
    /// 添加关系：发给对方
    /// </summary>
    public class RelationAdd : PackBaseS2C
    {
        public long event_idx;
        public PlayerIDName player_id = new PlayerIDName();//请求者id
        public string message;
        public eRelationFlag flag;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            event_idx = by.ReadLong();
            player_id.Read(by);
            flag = (eRelationFlag)(by.ReadByte());
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(event_idx);
            player_id.Write(by);
            by.WriteByte((byte)flag);
        }
    }
    /// <summary>
    /// 移除关系：分别发给双方
    /// </summary>
    public class RelationRemove : PackBaseS2C
    {
        public long target_char_idx;//需要移除的角色id

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
    /// 关系列表：分别发给双方
    /// </summary>
    public class RelationList : PackBaseS2C
    {
        public List<RelationInfo> list = new List<RelationInfo>();//新增关系列表
        public override void Init()
        {
            list.Clear();
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            list.Clear();
            ushort count = by.ReadUShort();
            for (int i = 0; i < count; ++i)
            {
                RelationInfo obj = new RelationInfo();
                obj.Read(by);
                list.Add(obj);
            }
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            ushort count = (ushort)list.Count;
            by.WriteUShort(count);
            for (int i = 0; i < count; ++i)
            {
                RelationInfo obj = list[i];
                obj.Write(by);
            }
        }
    }
    /// <summary>
    /// 好友赠送：发给对方
    /// </summary>
    public class RelationGive : PackBaseS2C
    {
        public PlayerIDName player_id = new PlayerIDName();//赠送者id
        public ItemID item_id = new ItemID();

        public override void Read(ByteArray by)
        {
            base.Read(by);
            player_id.Read(by);
            item_id.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            player_id.Write(by);
            item_id.Write(by);
        }
    }
    #endregion

    #region 次数
    /// <summary>
    /// 次数列表
    /// </summary>
    public class CounterList : PackBaseS2C
    {
        public List<CounterInfo> list = new List<CounterInfo>(); 

        public override void Init()
        {
            list.Clear();
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            list.Clear();
            ushort count = by.ReadUShort();
            for (int i = 0; i < count; ++i)
            {
                CounterInfo obj = new CounterInfo();
                obj.Read(by);
                list.Add(obj);
            }
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            ushort count = (ushort)list.Count;
            by.WriteUShort(count);
            for (int i = 0; i < count; ++i)
            {
                CounterInfo obj = list[i];
                obj.Write(by);
            }
        }
    }
    /// <summary>
    /// 次数更改
    /// </summary>
    public class CounterModify : PackBaseS2C
    {
        public eCounterType type;
        public ushort value;

        public override void Read(ByteArray by)
        {
            base.Read(by);
            type = (eCounterType)by.ReadByte();
            value = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte((byte)type);
            by.WriteUShort(value);
        }
    }
    #endregion

    #region 聊天
    /// <summary>
    /// 聊天
    /// </summary>
    public class ChatRecv : PackBaseS2C
    {
        public eChatType type;
        public CharacterIdxName sender;
        public string chat_content;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            type = (eChatType)(by.ReadByte());
            sender.Read(by);
            chat_content = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte((byte)type);
            sender.Write(by);
            by.WriteString(chat_content);
        }
    }
    /// <summary>
    /// 聊天结果
    /// </summary>
    public class ChatResult : PackBaseS2C
    {
        public eChatError error;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            error = (eChatError)(by.ReadByte());
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte((byte)error);
        }
    }
    #endregion
}
