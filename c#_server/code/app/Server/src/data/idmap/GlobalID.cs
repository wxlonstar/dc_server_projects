using System;
using System.Collections.Generic;

namespace dc
{
    public class GlobalID
    {
        /// <summary>
        /// 上报给ws当前在线时间间隔(单位：秒)
        /// </summary>
        public static int UPLOAD_ONLINE_COUNT_TIME = 60 * 1;

        ///// <summary>
        ///// 多长时间自动连接战斗服(单位：秒)
        ///// 战斗服宕机后，玩家会自动连接其他战斗服
        ///// 注意这个时间不能设置太短，否则会出现一开战斗服，所有玩家同时连接到一个战斗服
        ///// </summary>
        //public static int CONNECT_FIGHT_SERVER_TIME = 30 * 1;
    }
}
