using System;
using System.Collections.Generic;

namespace dc
{
    public class relation
    {
        /// <summary>
        /// 好友人数上限
        /// </summary>
        public const int PRIVATE_MAX_FRIEND_COUNT = 200;
        /// <summary>
        /// 黑名单人数上限
        /// </summary>
        public const int PRIVATE_MAX_BLOCK_COUNT = 200;
        /// <summary>
        /// 多久后可以再次添加同一个好友，防止频繁添加，单位秒
        /// </summary>
        public const int ADD_RELATION_TIME_OFFSET =  60;

        /// <summary>
        /// 更新db频繁，单位秒
        /// </summary>
        public const int DB_UPDATE_TIME_OFFSET = 60 * 5;
    }
    /// <summary>
    /// 关系数据
    /// </summary>
    public class RelationInfo : ISerializeObject
    {
        public long char_idx;       // 角色id
        public ushort spid;         // 渠道id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public ushort ws_id;         // 所在大区
        public eRelationFlag flags; // 关系标记
        public uint model_idx;      // 模型ID
        public byte job;            // 职业
        public ushort level;        // 角色等级
        public uint exp;            // 当前经验
        public uint gold;           // 金币（点卷）
        public uint coin;           // 游戏币(铜币)
        public uint vip_grade;      // vip等级

        public void Read(ByteArray by)
        {
            int start_pos = by.Head;
            char_idx = by.ReadLong();
            spid = by.ReadUShort();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            ws_id = by.ReadUShort();
            flags = (eRelationFlag)by.ReadByte();
            model_idx = by.ReadUInt();
            job = by.ReadByte();
            level = by.ReadUShort();
            exp = by.ReadUInt();
            gold = by.ReadUInt();
            coin = by.ReadUInt();
            vip_grade = by.ReadUInt();
            by.SetHead(start_pos + 200);//每个关系预留200字节，方便后期版本兼容
        }
        public void Write(ByteArray by)
        {
            int start_pos = by.Tail;
            by.WriteLong(char_idx);
            by.WriteUShort(spid);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUShort(ws_id);
            by.WriteByte((byte)flags);
            by.WriteUInt(model_idx);
            by.WriteByte(job);
            by.WriteUShort(level);
            by.WriteUInt(exp);
            by.WriteUInt(gold);
            by.WriteUInt(coin);
            by.WriteUInt(vip_grade);
            by.WriteEmpty(200 - (by.Tail - start_pos));
        }
        public void Copy(PlayerInfoForGL info)
        {
            char_idx = info.char_idx;
            spid = info.spid;
            char_name = info.char_name;
            char_type = info.char_type;
            ws_id = info.ws_id;
            //flags = eRelationFlag.Invalid;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            gold = info.gold;
            coin = info.coin;
            vip_grade = info.vip_grade;
        }
        public void Copy(RelationInfo info)
        {
            char_idx = info.char_idx;
            spid = info.spid;
            char_name = info.char_name;
            char_type = info.char_type;
            ws_id = info.ws_id;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            gold = info.gold;
            coin = info.coin;
            vip_grade = info.vip_grade;
        }
        public static int BaseSize
        {
            get
            {
                return (
                    sizeof(long) + 
                    sizeof(ushort)+
                    sizeof(byte)+
                    sizeof(byte)+
                    sizeof(uint)+
                    sizeof(byte)+
                    sizeof(ushort)+
                    sizeof(uint)+
                    sizeof(uint)+
                    sizeof(uint)+
                    sizeof(uint)
                    );
            }
        }
    }
    /// <summary>
    /// 添加关系目标
    /// </summary>
    public struct RelationAddTarget : ISerializeObject
    {
        public eRelationAddType type;
        public long char_idx;       //根据id
        public string char_name;    //根据名称
        public void Read(ByteArray by)
        {
            type = (eRelationAddType)(by.ReadByte());
            switch (type)
            {
                case eRelationAddType.Idx: char_idx = by.ReadLong(); break;
                case eRelationAddType.Name: char_name = by.ReadString(); break;
            }
        }
        public void Write(ByteArray by)
        {
            by.WriteByte((byte)type);
            switch (type)
            {
                case eRelationAddType.Idx: by.WriteLong(char_idx); break;
                case eRelationAddType.Name: by.WriteString(char_name); break;
            }
        }
        public static bool operator ==(RelationAddTarget target1, RelationAddTarget target2)
        {
            if(target1.type == target2.type) 
            {
                if ((target1.type == eRelationAddType.Idx && target1.char_idx == target2.char_idx) ||
                    (target1.type == eRelationAddType.Name && target1.char_name == target2.char_name))
                    return true;
            }
            return false;
        }
        public static bool operator !=(RelationAddTarget target1, RelationAddTarget target2)
        {
            return !(target1 == target2);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    /// <summary>
    /// 事件信息
    /// </summary>
    public struct RelationEventInfo : ISerializeObject
    {
        public long event_idx;                      // id
        public long target_char_idx;                // 目标id
        public long source_char_idx;                // 源角色id
        private eRelationEvent m_event_type;        // 事件类型
        public RelationEventContent bin_content;    // 内容

        public void Read(ByteArray by)
        {
            event_idx = by.ReadLong();
            target_char_idx = by.ReadLong();
            source_char_idx = by.ReadLong();
            event_type = (eRelationEvent)by.ReadByte();
            bin_content.Read(by);
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(event_idx);
            by.WriteLong(target_char_idx);
            by.WriteLong(source_char_idx);
            by.WriteByte((byte)event_type);
            bin_content.event_type = event_type;
            bin_content.Write(by);
        }
        public eRelationEvent event_type
        {
            get { return m_event_type; }
            set 
            { 
                m_event_type = value;
                bin_content.event_type = value;
            }
        }
    }
    /// <summary>
    /// 事件内容
    /// </summary>
    public struct RelationEventContent : ISerializeObject
    {
        /// <summary>
        /// 添加
        /// </summary>
        public struct AddContent : ISerializeObject
        {
            public string char_name;
            public string message;
            public eRelationFlag flag; //关系方式

            public void Read(ByteArray by)
            {
                char_name = by.ReadString();
                message = by.ReadString();
                flag = (eRelationFlag)by.ReadByte();
            }
            public void Write(ByteArray by)
            {
                by.WriteString(char_name);
                by.WriteString(message);
                by.WriteByte((byte)flag);
            }
        }
        /// <summary>
        /// 移除
        /// </summary>
        public struct DeleteContent : ISerializeObject
        {
            public void Read(ByteArray by)
            {
            }
            public void Write(ByteArray by)
            {
            }
        }
        /// <summary>
        /// 同意
        /// </summary>
        public struct AgreeContent : ISerializeObject
        {
            public eRelationFlag flag;
            public eRelationApplyCmd cmd;

            public void Read(ByteArray by)
            {
                flag = (eRelationFlag)(by.ReadByte());
                cmd = (eRelationApplyCmd)(by.ReadByte());
            }
            public void Write(ByteArray by)
            {
                by.WriteByte((byte)flag);
                by.WriteByte((byte)cmd);
            }
        }
        /// <summary>
        /// 赠送
        /// </summary>
        public struct GiveContent : ISerializeObject
        {
            public string char_name;
            public ItemID item_id;
            public void Read(ByteArray by)
            {
                char_name = by.ReadString();
                item_id.Read(by);
            }
            public void Write(ByteArray by)
            {
                by.WriteString(char_name);
                item_id.Write(by);
            }
        }

        public eRelationEvent event_type;          // 事件类型
        public AddContent bin_add_content;
        public DeleteContent bin_delete_content;
        public AgreeContent bin_agree_content;
        public GiveContent bin_give_content;
        public void Read(ByteArray by)
        {
            switch (event_type)
            {
                case eRelationEvent.Add: bin_add_content.Read(by); break;
                case eRelationEvent.Delete: bin_delete_content.Read(by); break;
                case eRelationEvent.Agree: bin_agree_content.Read(by); break;
                case eRelationEvent.Give: bin_give_content.Read(by); break;
            }
        }
        public void Write(ByteArray by)
        {
            switch (event_type)
            {
                case eRelationEvent.Add: bin_add_content.Write(by); break;
                case eRelationEvent.Delete: bin_delete_content.Write(by); break;
                case eRelationEvent.Agree: bin_agree_content.Write(by); break;
                case eRelationEvent.Give: bin_give_content.Write(by); break;
            }
        }
    }

    /// <summary>
    /// 关系标记位
    /// </summary>
    public enum eRelationFlag
    {
        Invalid    = 0,		    // 无
        Friend     = 1,		    // 好友
        Block      = 2,		    // 黑名单

        End,
    }
    /// <summary>
    /// 添加关系方式
    /// </summary>
    public enum eRelationAddType
    {
        Idx,        //根据id
        Name,       //根据名称
    }
    /// <summary>
    /// 请求好友操作
    /// </summary>
    public enum eRelationApplyCmd
    {
        Agree,      //同意
        Reject,     //拒绝
    }
    /// <summary>
    /// 事件
    /// </summary>
    public enum eRelationEvent
    {
        Add,        //请求添加
        Delete,     //移除关系
        Agree,      //对方同意
        Give,       //赠送
    }
}
