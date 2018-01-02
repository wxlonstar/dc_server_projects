using System;
using System.Collections.Generic;

namespace dc
{
    public class GlobalID
    {
        /// <summary>
        /// 最大缓存玩家数据，超过会对下线玩家数据清理
        /// </summary>
        public static int MAX_CACHE_UNIT_COUNT = 500000;
        /// <summary>
        /// 单次最多回收的玩家数量
        /// </summary>
        public static int TOTAL_RELEASE_CACHE_UNIT_PER = 1000;
    }
}
