using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 服务器事件定义
    /// @author hannibal
    /// @time 2016-8-18
    /// </summary>
    public class EventID
    {
        /// <summary>
        /// 成功连接世界服
        /// </summary>
        public const string NET_CONNECTED_WORLD_SRV = "NET_CONNECTED_WORLD_SRV";
        /// <summary>
        /// 与世界服断开连接
        /// </summary>
        public const string NET_DISCONNECT_WORLD_SRV = "NET_DISCONNECT_WORLD_SRV";
        /// <summary>
        /// 与战斗服断开连接
        /// </summary>
        public const string NET_DISCONNECT_FIGHT_SRV = "NET_DISCONNECT_FIGHT_SRV";

        /// <summary>
        /// 玩家进入游戏
        /// </summary>
        public const string PLAYER_ENTER_GAME = "PLAYER_ENTER_GAME";
        /// <summary>
        /// 玩家离开游戏
        /// </summary>
        public const string PLAYER_LEAVE_GAME = "PLAYER_LEAVE_GAME";

        /// <summary>
        /// 整点报时：小时为单位，每天0-24时报时
        /// </summary>
        public const string INTEGRAL_HOUR_TIMER = "INTEGRAL_HOUR_TIMER";
        /// <summary>
        /// 指定日期的时间报时
        /// </summary>
        public const string INTEGRAL_DATE_HOUR_TIMER = "INTEGRAL_DATE_HOUR_TIMER";

        /// <summary>
        /// 新DB事件，注：
        /// 1.只有需要及时处理的事件才能派发这个事件，过高的频率调用会导致db占用过高；
        /// 2.内部会定时读取db是否有新事件，减少db开销
        /// </summary>
        public const string DB_NEW_EVENT = "DB_NEW_EVENT";
    }
}
