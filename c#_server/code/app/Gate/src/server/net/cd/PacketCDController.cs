using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 协议cd控制
    /// @author hannibal
    /// @time 2016-9-20
    /// </summary>
    public class PacketCDVerify
    {
        private Dictionary<ushort, long> m_cooldowns = new Dictionary<ushort, long>();

        /// <summary>
        /// 判断协议cd是否ok
        /// </summary>
        public bool CheckCooldown(ushort msg_idx)
        {
            StdPacketCDInfo cd_info = PacketCDConfig.Instance.GetInfo(msg_idx);
            if (cd_info == null) return true;

            long old_time = 0;
            if (!m_cooldowns.TryGetValue(msg_idx, out old_time))
            {
                m_cooldowns.Add(msg_idx, Time.timeSinceStartup);
                return true;
            }

            if (Time.timeSinceStartup - old_time >= cd_info.cd)
            {
                m_cooldowns[msg_idx] = Time.timeSinceStartup;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            m_cooldowns.Clear();
        }
    }
}
