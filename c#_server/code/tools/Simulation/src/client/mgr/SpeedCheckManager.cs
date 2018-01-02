using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 防加速
    /// @author hannibal
    /// @time 2016-10-25
    /// </summary>
    public class SpeedCheckManager : Singleton<SpeedCheckManager>
    {
        private int m_check_sn = 0;

        /// <summary>
        /// 接受到加速检测包
        /// </summary>
        /// <param name="time_offset"></param>
        /// <param name="check_sn"></param>
        public void OnRecvCheckGrap(int check_sn, int time_offset)
        {
            if (m_check_sn > 0)
            {
                SendCheck();
            }

            m_check_sn = check_sn;
            Log.Debug("收到：" + Time.second_time + " " + time_offset);
            TimerManager.Instance.AddOnce(time_offset, (timer_id, param) =>
            {
                Log.Debug("触发：" + Time.second_time);
                this.SendCheck();
            });
        }
        private void SendCheck()
        {
            if (m_check_sn == 0) return;

            ServerMsgSend.SendSpeedCheck(m_check_sn);

            m_check_sn = 0;
        }
    }
}
