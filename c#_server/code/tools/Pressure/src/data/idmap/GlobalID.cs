using System;
using System.Collections.Generic;

namespace dc
{
    public class GlobalID
    {
        /// <summary>
        /// 主版本号：有效范围(0-255),重大更新时修改
        /// </summary>
        public static uint VERSION_MAIN = 0;
        /// <summary>
        /// 子版本号：有效范围(0-255),出现新功能时增加
        /// </summary>
        public static uint VERSION_SUB = 1;
        /// <summary>
        /// 修正版本号：有效范围(0-65535),修复以前发布的程序中的bug
        /// </summary>
        public static uint VERSION_REVISION = 1;

        /// <summary>
        /// 是否消息启用加密
        /// </summary>
        public static bool ENABLE_PACKET_ENCRYPT = true;

        /// <summary>
        /// 加密key
        /// </summary>
        public static ushort ENCRYPT_KEY = 0;

        public static uint GetVersion()
        {
            return (VERSION_MAIN << 24 | VERSION_SUB << 16 | VERSION_REVISION);
        }
    }
}
