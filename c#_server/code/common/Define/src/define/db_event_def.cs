using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 通过db转存的游戏事件
    /// @author hannibal
    /// @time 2016-11-1
    /// </summary>
    public class db_event
    {        
        /// <summary>
        /// 读取db频繁，单位秒
        /// </summary>
        public const int DB_UPDATE_TIME_OFFSET = 60 * 5;
    }
    /// <summary>
    /// 事件信息
    /// </summary>
    public struct DBEventInfo : ISerializeObject
    {
        public long event_idx;                      // id
        public long target_char_idx;                // 目标id
        public long source_char_idx;                // 源角色id
        private eDBEventType m_event_type;          // 事件类型
        public DBEventContent bin_content;          // 内容

        public void Read(ByteArray by)
        {
            event_idx = by.ReadLong();
            target_char_idx = by.ReadLong();
            source_char_idx = by.ReadLong();
            event_type = (eDBEventType)by.ReadByte();
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
        public eDBEventType event_type
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
    public struct DBEventContent : ISerializeObject
    {
        /// <summary>
        /// 通用类型
        /// </summary>
        public struct NormalContent : ISerializeObject
        {
            public string data;

            public void Read(ByteArray by)
            {
                data = by.ReadString();
            }
            public void Write(ByteArray by)
            {
                by.WriteString(data);
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

        public eDBEventType event_type;          // 事件类型
        public NormalContent bin_normal_content;
        public GiveContent bin_give_content;
        public void Read(ByteArray by)
        {
            switch (event_type)
            {
                case eDBEventType.Test: bin_normal_content.Read(by); break;
                case eDBEventType.Give: bin_give_content.Read(by); break;
            }
        }
        public void Write(ByteArray by)
        {
            switch (event_type)
            {
                case eDBEventType.Test: bin_normal_content.Write(by); break;
                case eDBEventType.Give: bin_give_content.Write(by); break;
            }
        }
    }
    /// <summary>
    /// 事件
    /// </summary>
    public enum eDBEventType
    {
        Test,       //测试
        Give,       //赠送
    }
}
