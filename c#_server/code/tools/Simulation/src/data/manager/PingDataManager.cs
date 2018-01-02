using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// Ping网络
    /// @author hannibal
    /// @time 2017-10-20
    /// </summary>
    public class PingDataManager : Singleton<PingDataManager>
    {
        private int m_timer_id = 0;
        private uint m_ping_type = 0;
        private uint m_packet_idx = 0;

        public void Setup()
        {
        }

        public void Destroy()
        {
            this.Stop();
        }

        public void Start(uint ping_type, int time_offset)
        {
            if (m_timer_id > 0) return;

            m_packet_idx = 0;
            m_ping_type = ping_type;
            m_timer_id = TimerManager.Instance.AddLoop(time_offset * 1000, 0, (timer_id, param)=>
            {
                if(Utils.HasFlag(m_ping_type, (uint)ePingType.Gate))
                {
                    ServerMsgSend.SendPingGS(++m_packet_idx);
                }
                if (Utils.HasFlag(m_ping_type, (uint)ePingType.Server))
                {
                    ServerMsgSend.SendPingSS(++m_packet_idx);
                }
                if (Utils.HasFlag(m_ping_type, (uint)ePingType.Fight))
                {
                    ServerMsgSend.SendPingFS(++m_packet_idx);
                }
                if (Utils.HasFlag(m_ping_type, (uint)ePingType.World))
                {
                    ServerMsgSend.SendPingWS(++m_packet_idx);
                }
                if (Utils.HasFlag(m_ping_type, (uint)ePingType.Global))
                {
                    ServerMsgSend.SendPingGL(++m_packet_idx);
                }
            });
        }

        public void Stop()
        {
            if(m_timer_id > 0)
            {
                TimerManager.Instance.RemoveTimer(m_timer_id);
                m_timer_id = 0;
            }
            m_ping_type = 0;
        }
    }
    public enum ePingType
    {
        None    = 0,
        Gate    = 0x01,
        Server  = 0x02,
        Fight   = 0x04,
        World   = 0x08,
        Global  = 0x10,
    }
}
