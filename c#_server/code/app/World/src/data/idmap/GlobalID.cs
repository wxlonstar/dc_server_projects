using System;
using System.Collections.Generic;

namespace dc
{
    public class GlobalID
    {
        /// <summary>
        /// 角色id初始值，真实的角色id会再乘以大区号，保证每个大区id不重复
        /// </summary>
        public static long INIT_CHAR_IDX = 1000000000;

        /// <summary>
        /// 关服等待时间(单位：秒)
        /// </summary>
        public static int TOTAL_WAIT_SHUTDOWN = 3;

        /// <summary>
        /// 检测玩家是否在线时间间隔(单位：秒)
        /// </summary>
        public static int TOTAL_CHECK_ONLINE_TIME = 60 * 10;

        /// <summary>
        /// 最大缓存账号数据
        /// </summary>
        public static int MAX_CACHE_ACCOUNT_COUNT = 100000;
        /// <summary>
        /// 单次最多回收的账号数量
        /// </summary>
        public static int TOTAL_RELEASE_CACHE_ACCOUNT_PER = 1000;
    }
}
