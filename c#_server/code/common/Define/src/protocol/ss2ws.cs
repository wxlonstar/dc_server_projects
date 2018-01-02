using System;
using System.Collections.Generic;

namespace dc.ss2ws
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_SS2WS;
        public const ushort LOGIN_CLIENT        = Begin + 1;        //告诉ws，有客户端连接
        public const ushort LOGOUT_CLIENT       = Begin + 2;        //登出
        public const ushort CLIENT_ONLINE       = Begin + 3;        //检测是否在线
        public const ushort ONLINE_COUNT        = Begin + 4;        //在线数
        
        public const ushort UNIT_MODIFY_INT     = Begin + 10;       //属性改变
        public const ushort UNIT_MODIFY_STRING  = Begin + 11;       //属性改变

        public const ushort CHAT_SEND           = Begin + 50;       //聊天

        public static void RegisterPools()
        {
            PacketPools.Register(LOGIN_CLIENT, "ss2ws.LoginClient");
            PacketPools.Register(LOGOUT_CLIENT, "ss2ws.LogoutClient");
            PacketPools.Register(CLIENT_ONLINE, "ss2ws.ClientOnline");
            PacketPools.Register(ONLINE_COUNT, "ss2ws.OnlineCount");

            PacketPools.Register(UNIT_MODIFY_INT, "ss2ws.NotifyUpdatePlayerAttribInteger");
            PacketPools.Register(UNIT_MODIFY_STRING, "ss2ws.NotifyUpdatePlayerAttribString");

            PacketPools.Register(CHAT_SEND, "ss2ws.ChatSend");
        }
    }
    #region 角色
    /// <summary>
    /// 进入游戏
    /// </summary>
    public class LoginClient : PackBaseS2S
    {
        public ClientUID client_uid;
        public PlayerInfoForWS data = new PlayerInfoForWS();

        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
            data.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
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
    /// 是否在线检测
    /// </summary>
    public class ClientOnline : PackBaseS2S
    {
        public long char_idx;
        public bool is_online;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            char_idx = by.ReadLong();
            is_online = by.ReadBool();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteLong(char_idx);
            by.WriteBool(is_online);
        }
    }
    /// <summary>
    /// 在线数量
    /// </summary>
    public class OnlineCount : PackBaseS2S
    {
        public ushort count;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            count = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort(count);
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

    #region 聊天
    /// <summary>
    /// 聊天
    /// </summary>
    public class ChatSend : PackBaseS2S
    {
        public eChatType type;
        public CharacterIdxName sender;
        public CharacterIdxOrName receiver;
        public string chat_content;
        public override void Read(ByteArray by)
        {
            base.Read(by);
            type = (eChatType)(by.ReadByte());
            sender.Read(by);
            receiver.Read(by);
            chat_content = by.ReadString();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteByte((byte)type);
            sender.Write(by);
            receiver.Write(by);
            by.WriteString(chat_content);
        }
    }
    #endregion
}
