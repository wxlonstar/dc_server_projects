using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 聊天
    /// @author hannibal
    /// @time 2017-8-14
    /// </summary>
    public class chat
    {
        /// <summary>
        /// 聊天内容最大值(字节)
        /// </summary>
        public const int LIMIT_CHAT_TEXT_LENGTH = 512;
    }
    /// <summary>
    /// 聊天类型
    /// </summary>
    public enum eChatType
    {
        // 系统频道(全世界范围)
        SYSTEM = 1,
        // 私聊频道(玩家间的)
        PRIVATE,
        // 世界频道
        WORLD,
        // 当前频道(同场景：以玩家所在位置，向周围广播，理论是可见玩家会收到)
        CURRENT,
        // 组队聊天
        GROUP,
        // 军团频道
        GUILD,
        // 战场聊天
        INSTANCE,
    }
    /// <summary>
    /// 聊天消息的发送者定义
    /// </summary>
    public enum eChatSender
    {
        // 游戏内系统
        SYSTEM = 1,
        // 后台系统(gm kf等)
        GM_SYSTEM = 2,
        // 游戏事件
        GAME_EVENT = 4,
    }
    /// <summary>
    /// 聊天错误原因
    /// </summary>
    public enum eChatError
	{
		// 无错误
		ERROR_NO_ERROR = 0,
		// 对方不在线
		ERROR_DST_OFFLINE,
		// 已经被对方屏蔽（黑名单）
		ERROR_DST_REFUSE,
	}
}
