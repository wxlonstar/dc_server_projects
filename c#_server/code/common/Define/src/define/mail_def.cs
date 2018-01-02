using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 邮件常量定义
    /// </summary>
    public class mail
    {				
        /// <summary>
        /// 主题最小长度
        /// </summary>
		public static int MIN_SUBJECT_LENGTH = 2;
		/// <summary>
        /// 主题最大长度
		/// </summary>
		public static int MAX_SUBJECT_LENGTH = 24;
		/// <summary>
        /// 正文最大长度
		/// </summary>
        public static int MAX_BODY_LENGTH = 300;
        /// <summary>
        /// 发送邮件最低角色等级
        /// </summary>
        public static int MIN_SEND_MAIL_LEVEL = 1;
        /// <summary>
        /// 未阅读邮件期限 3天(s)
        /// </summary>
		public static int MAX_UNREAD_EXPIRE_TIME = 3*24*60*60;
		/// <summary>
        /// 已阅读邮件期限 3天(s)
		/// </summary>
		public static int MAX_READED_EXPIRE_TIME = 3*24*60*60;
		/// <summary>
        /// 赠送邮件附件领取期限 7天(s)
		/// </summary>
		public static int MAX_PRESENT_EXPIRE_TIME= 7*24*60*60;
    }
    
    /// <summary>
    /// 单封邮件标题信息
    /// </summary>
    public struct MailTitleInfo : ISerializeObject
    {
        // 唯一id
        public long mail_idx;
        // 对应eMailType
        public eMailType mail_type;

        // 发送者
        public long sender_idx;
        public string sender_name;

        // 发送信件时间
        public long send_time;
        // 期限 为0表示永不过期
        public int expire_time;
        // 对应eMailFlags
        public uint flags;
        // 主题
        public string subject;

        public void Read(ByteArray by)
        {
            mail_idx = by.ReadLong();
            mail_type = (eMailType)by.ReadByte();
            sender_idx = by.ReadLong();
            sender_name = by.ReadString();
            send_time = by.ReadLong();
            expire_time = by.ReadInt();
            flags = by.ReadUInt();
            subject = by.ReadString();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(mail_idx);
            by.WriteByte((byte)mail_type);
            by.WriteLong(sender_idx);
            by.WriteString(sender_name);
            by.WriteLong(send_time);
            by.WriteInt(expire_time);
            by.WriteUInt(flags);
            by.WriteString(subject);
        }
        public void CopyFromInfo(MailInfo mail_info)
        {
            mail_idx = mail_info.mail_idx;
            mail_type = mail_info.mail_type;
            sender_idx = mail_info.sender_idx;
            sender_name = mail_info.sender_name;
            send_time = mail_info.send_time;
            expire_time = mail_info.expire_time;
            flags = mail_info.flags;
            subject = mail_info.subject;
        }
    }
        
    /// <summary>
    /// 玩家领取过的邮件
    /// </summary>
    public struct MailCharRecvs : ISerializeObject
    {
        // 领取过的邮件
        private List<long> recv_mails;

        public void Read(ByteArray by)
        {
            ushort count = by.ReadUShort();
            for (int i = 0; i < count; ++i)
            {
                long mail_idx = by.ReadLong();
                this.Add(mail_idx);
            }
        }
        public void Write(ByteArray by)
        {
            ushort count = (ushort)recv_mails.Count;
            by.WriteUShort(count);
            for (int i = 0; i < count; ++i)
            {
                by.WriteLong(recv_mails[i]);
            }
        }
        public bool Contains(long mail_idx)
        {
            if (recv_mails != null && recv_mails.Contains(mail_idx))
                return true;
            return false;
        }
        public void Add(long mail_idx)
        {
            if (recv_mails == null) recv_mails = new List<long>();
            if (!recv_mails.Contains(mail_idx))
                recv_mails.Add(mail_idx);
        }
    }
    /// <summary>
    /// 单封邮件信息
    /// </summary>
    public class MailInfo : ISerializeObject, IPoolsObject
    {
        // 唯一id
        public long mail_idx;
		// 对应eMailType
        public eMailType mail_type;
        // 渠道id
        public ushort spid;

		// 收件人
        public long receiver_idx;
		// 发送者
        public long sender_idx;
        public string sender_name;

		// 发送信件时间
        public long send_time;
		// 期限 为0表示永不过期
        public int expire_time;
		// 投递送到时的时间(路途时间) 为0表示无投递时间
        public int delivery_time;
        // 对应eMailFlags
        public uint flags;
        // 主题
        public string subject;

        // 内容
        public MailContent bin_mail_content = new MailContent();

        public void Init()
        {
        }
        public void Copy(MailInfo mail_info)
        {
            mail_idx = mail_info.mail_idx;
            mail_type = mail_info.mail_type;
            spid = mail_info.spid;
            receiver_idx = mail_info.receiver_idx;
            sender_idx = mail_info.sender_idx;
            sender_name = mail_info.sender_name;
            send_time = mail_info.send_time;
            expire_time = mail_info.expire_time;
            delivery_time = mail_info.delivery_time;
            flags = mail_info.flags;
            subject = mail_info.subject;
            bin_mail_content = mail_info.bin_mail_content;
        }
        public void Read(ByteArray by)
        {
            mail_idx = by.ReadLong();
            mail_type = (eMailType)by.ReadByte();
            spid = by.ReadUShort();
            receiver_idx = by.ReadLong();
            sender_idx = by.ReadLong();
            sender_name = by.ReadString();
            send_time = by.ReadLong();
            expire_time = by.ReadInt();
            delivery_time = by.ReadInt();
            flags = by.ReadUInt();
            subject = by.ReadString();
            bin_mail_content.Read(by);
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(mail_idx);
            by.WriteByte((byte)mail_type);
            by.WriteUShort(spid);
            by.WriteLong(receiver_idx);
            by.WriteLong(sender_idx);
            by.WriteString(sender_name);
            by.WriteLong(send_time);
            by.WriteInt(expire_time);
            by.WriteInt(delivery_time);
            by.WriteUInt(flags);
            by.WriteString(subject);
            bin_mail_content.Write(by);
        }
    }
    /// <summary>
    /// 邮件内容
    /// </summary>
    public struct MailContent : ISerializeObject
    {
        // 内容类型，对应eMailContentType
        public eMailContentType content_type;
        public MailNormalContent bin_normal_content;
        public MailNoticeContent bin_notice_content;
        public MailItemContent bin_item_content;
        public MailModContent bin_mod_content;
        public void Read(ByteArray by)
        {
            content_type = (eMailContentType)by.ReadByte();
            switch (content_type)
            {
                case eMailContentType.NORMAL: bin_normal_content.Read(by); break;
                case eMailContentType.NOTICE: bin_notice_content.Read(by); break;
                case eMailContentType.SEND_ITEMS: bin_item_content.Read(by); break;
                case eMailContentType.MOD: bin_mod_content.Read(by); break;
            }
        }
        public void Write(ByteArray by)
        {
            by.WriteByte((byte)content_type);
            switch (content_type)
            {
                case eMailContentType.NORMAL: bin_normal_content.Write(by); break;
                case eMailContentType.NOTICE: bin_notice_content.Write(by); break;
                case eMailContentType.SEND_ITEMS: bin_item_content.Write(by); break;
                case eMailContentType.MOD: bin_mod_content.Write(by); break;
            }
        }
    }
    /// <summary>
    /// 普通邮件内容
    /// </summary>
    public struct MailNormalContent : ISerializeObject
    {
		// 正文
        public string content;
        public void Read(ByteArray by)
        {
            content = by.ReadString();
        }
        public void Write(ByteArray by)
        {
            by.WriteString(content);
        }
    }
    /// <summary>
    /// 通知邮件内容
    /// </summary>
    public struct MailNoticeContent : ISerializeObject
    {
        /// <summary>
        /// 军团-申请军团成功
        /// </summary>
        public struct NtcGuildAppSuccess : ISerializeObject
        {
            public string name; //军团名称
            public void Read(ByteArray by)
            {
                name = by.ReadString();
            }
            public void Write(ByteArray by)
            {
                by.WriteString(name);
            }
        }
        /// <summary>
        /// 军团-军团申请失败
        /// </summary>
        public struct NtcGuildAppFail : ISerializeObject
        {
			public string name;
            public byte type;   //拒绝1 或者 超时2
            public void Read(ByteArray by)
            {
                name = by.ReadString();
                type = by.ReadByte();
            }
            public void Write(ByteArray by)
            {
                by.WriteString(name);
                by.WriteByte(type);
            }
		}
        /// <summary>
        /// 军团-军团解散
        /// </summary>
        public struct NtcGuildDismiss : ISerializeObject
        {
            public string name;
            public void Read(ByteArray by)
            {
                name = by.ReadString();
            }
            public void Write(ByteArray by)
            {
                by.WriteString(name);
            }
		}
        /// <summary>
        /// 军团-军团踢出
        /// </summary>
        public struct NtcGuildOut : ISerializeObject
        {
            public string name;//军团名称
            public void Read(ByteArray by)
            {
                name = by.ReadString();
            }
            public void Write(ByteArray by)
            {
                by.WriteString(name);
            }
		}
        /// <summary>
        /// 军团-军团退出
        /// </summary>
        public struct NtcGuildExit : ISerializeObject
        {
            public string name;//军团名称
            public void Read(ByteArray by)
            {
                name = by.ReadString();
            }
            public void Write(ByteArray by)
            {
                by.WriteString(name);
            }
		}
        // 通知类型
        public eMailNoticeType notice_type;
        // 通知内容
        public NtcGuildAppSuccess notice_guide_app_success;
        public NtcGuildAppFail notice_guide_app_fail;
        public NtcGuildDismiss notice_guide_dismiss;
        public NtcGuildOut notice_guide_out;
        public NtcGuildExit notice_guide_exit;

        public void Read(ByteArray by)
        {
            notice_type = (eMailNoticeType)by.ReadByte();
            switch (notice_type)
            {
                case eMailNoticeType.GuildAppSucc: notice_guide_app_success.Read(by); break;
                case eMailNoticeType.GuildAppFail: notice_guide_app_fail.Read(by); break;
                case eMailNoticeType.GuildDismiss: notice_guide_dismiss.Read(by); break;
                case eMailNoticeType.GuildOut: notice_guide_out.Read(by); break;
                case eMailNoticeType.GuildExit: notice_guide_exit.Read(by); break;
                default: break;
            }
        }
        public void Write(ByteArray by)
        {
            by.WriteByte((byte)notice_type);
            switch (notice_type)
            {
                case eMailNoticeType.GuildAppSucc: notice_guide_app_success.Write(by); break;
                case eMailNoticeType.GuildAppFail: notice_guide_app_fail.Write(by); break;
                case eMailNoticeType.GuildDismiss: notice_guide_dismiss.Write(by); break;
                case eMailNoticeType.GuildOut: notice_guide_out.Write(by); break;
                case eMailNoticeType.GuildExit: notice_guide_exit.Write(by); break;
                default:  break;
            }
        }
    }
    /// <summary>
    /// 物品邮件内容
    /// </summary>
    public struct MailItemContent : ISerializeObject
    {
        // 主题
        public string subject;
        // 正文
        public string content;
        // 物品列表
        public List<ItemID> items;

        public void Read(ByteArray by)
        {
            subject = by.ReadString();
            content = by.ReadString();
            ushort count = by.ReadUShort();
            for (int i = 0; i < count; ++i)
            {
                ItemID obj = new ItemID();
                obj.Read(by);
                this.Add(obj);
            }
        }
        public void Write(ByteArray by)
        {
            by.WriteString(subject);
            by.WriteString(content);
            ushort count = (ushort)items.Count;
            by.WriteUShort(count);
            for (int i = 0; i < count; ++i)
            {
                ItemID obj = items[i];
                obj.Write(by);
            }
        }
        public void Add(ItemID obj)
        {
            if (items == null) items = new List<ItemID>();
            items.Add(obj);
        }
    }
    /// <summary>
    /// 修改玩家属性：系统内部使用
    /// </summary>
    public struct MailModContent : ISerializeObject
    {
        public struct ModInfo
        {
            public eUnitModType type;
            public long value_i;
            public string value_str;
            public void Read(ByteArray by)
            {
                type = (eUnitModType)by.ReadUShort();
                switch(type)
                {
                    case eUnitModType.UMT_char_name:
                        value_str = by.ReadString();
                        break;
                    default:
                        value_i = by.ReadLong();
                        break;
                }
            }
            public void Write(ByteArray by)
            {
                by.WriteUShort((ushort)type);
                switch (type)
                {
                    case eUnitModType.UMT_char_name:
                        by.WriteString(value_str);
                        break;
                    default:
                        by.WriteLong(value_i);
                        break;
                }
            }
        }
        // 列表
        public List<ModInfo> mods;
        public void Read(ByteArray by)
        {
            mods = new List<ModInfo>();
            ushort count = by.ReadUShort();
            for (int i = 0; i < count; ++i)
            {
                ModInfo obj = new ModInfo();
                obj.Read(by);
                mods.Add(obj);
            }
        }
        public void Write(ByteArray by)
        {
            ushort count = (ushort)mods.Count;
            by.WriteUShort(count);
            for (int i = 0; i < count; ++i)
            {
                ModInfo obj = mods[i];
                obj.Write(by);
            }
        }
    }
    /// <summary>
    /// 写邮件结构
    /// </summary>
    public struct MailWriteInfo : ISerializeObject
    {
        public CharacterIdxOrName receiver;

        public string subject;  //主题
        public string content;  //内容

        public void Read(ByteArray by)
        {
            receiver.Read(by);
            subject = by.ReadString();
            content = by.ReadString();
        }
        public void Write(ByteArray by)
        {
            receiver.Write(by);
            by.WriteString(subject);
            by.WriteString(content);
        }
    }
    /// <summary>
    /// 邮件类型
    /// </summary>
    public enum eMailType
	{
		// 普通邮件
		NORMAL,
        // 系统内部用(一般用来系统对离线玩家数据进行数据预操作)
        SYSTEM_INTERNAL,
		// 系统消息
		NOTICE,
		// 其他系统
		SYSTEM,
	}
    /// <summary>
    /// 邮件标志
    /// </summary>
    public enum eMailFlags
    {
       //			 Hex			Decimal			Bit  Comments
       NONE          = 0x00000000,	// 0,			01
       READED        = 0x00000001,	// 1,			02	已读
       CARRY_ITEM    = 0x00000010,	// 2			06	携带了物品
    }
    /// <summary>
    /// 内容类型
    /// </summary>
    public enum eMailContentType
    {
        // 普通邮件数据
        NORMAL,
        // 系统通知
        NOTICE,
        // 发送物品
        SEND_ITEMS,
        // 修改属性，用于对离线玩家登陆后的操作
        MOD,

        MAX,
    }
    /// <summary>
    /// 通知邮件类型
    /// </summary>
    public enum eMailNoticeType
    {
        GuildAppSucc,				// 军团-军团申请成功
		GuildAppFail,				// 军团-军团申请失败
		GuildDismiss,				// 军团-军团解散
		GuildOut,					// 军团-军团踢出
		GuildExit,					// 军团-军团退出
    }

    /// <summary>
    /// 邮件常用操作
    /// </summary>
    public enum eMailCommandType
    {
        INVALID = 0,
        // 发送邮件
        WRITE_MAIL,
        // 读取邮件
        READ_MAIL,
        // 删除邮件
        DELETE_MAIL,
        // 收取整个邮件(收取后删除邮件)
        TAKE_MAIL,
        // 收取邮件以及附件但不删除
        TAKE_MAIL_BUT_RETAIN,
    }

    /// <summary>
    /// 邮件操作错误类型定义
    /// </summary>
    public enum eMailCommandError
    {
        // 无错误
        NONE = 0,
        // 不能发送给自己
        CANNOT_SEND_TO_SELF,
        // 收件人未找到
        RECIPIENT_NOT_FOUND,
        // 内部错误
        INTERNAL_ERROR,
        // 不能拥有此类物品（达到拥有上限）
        CANT_CARRY_MORE_OF_THIS,
        // 背包没有空闲的位置
        ERROR_NOT_NOUGH_FREESLOT,
    }
}
