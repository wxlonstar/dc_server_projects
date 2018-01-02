using System;
using System.Collections.Generic;

namespace dc
{
    public class GlobalID
    {
        /// <summary>
        /// 踢掉无效连接时间(单位：秒)
        /// </summary>
        public static int KICKOUT_INVALID_SESSION_TIME = 5;
        /// <summary>
        /// 上报给ws当前在线时间间隔(单位：秒)
        /// </summary>
        public static int UPLOAD_ONLINE_COUNT_TIME = 60 * 1;

        /// <summary>
        /// 是否消息启用加密
        /// </summary>
        public static bool ENABLE_PACKET_ENCRYPT = true;

        #region 版本
        /// <summary>
        /// 主版本号：有效范围(0-255),重大更新时修改；前段主版本必须一样才能进游戏(暂定:具体规则可以根据游戏情况定)
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
        /// 客户端版本：登录时，保存的客户端版本；可用于版本兼容
        /// </summary>
        private static uint m_client_version = 0;

        /// <summary>
        /// 完整版本号
        /// </summary>
        /// <returns></returns>
        public static uint GetVersion()
        {
            return (VERSION_MAIN << 24 | VERSION_SUB << 16 | VERSION_REVISION);
        }
        /// <summary>
        /// 拆解版本号
        /// </summary>
        /// <param name="version">完整版本号</param>
        /// <param name="main_version">主版本</param>
        /// <param name="sub_version">子版本</param>
        /// <param name="revision_version">修订版</param>
        public static void SplitVersion(uint version, out byte main_version, out byte sub_version, out ushort revision_version)
        {
            main_version = (byte)((version >> 24) & 0x000000ff);
            sub_version = (byte)((version >> 16) & 0x000000ff);
            revision_version = (ushort)(version & 0x0000ffff);
        }

        /// <summary>
        /// 客户端版本号
        /// </summary>
        /// <returns></returns>
        public static void SetClientVersion(uint v)
        {
            m_client_version = v;
        }
        public static uint GetClientVersion()
        {
            return m_client_version;
        }
        /// <summary>
        /// 客户端主版本号
        /// </summary>
        /// <returns></returns>
        public static byte GetClientMainVersion()
        {
            return (byte)((m_client_version >> 24) & 0x000000ff);
        }
        /// <summary>
        /// 客户端子版本号
        /// </summary>
        /// <returns></returns>
        public static uint GetClientSubVersion()
        {
            return (byte)((m_client_version >> 16) & 0x000000ff);
        }
        /// <summary>
        /// 客户端修订版本号
        /// </summary>
        /// <returns></returns>
        public static uint GetClientRevisionVersion()
        {
            return (ushort)(m_client_version & 0x0000ffff);
        }
        #endregion
    }
}
