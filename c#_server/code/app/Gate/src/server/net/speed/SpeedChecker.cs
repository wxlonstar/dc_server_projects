using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 防加速或减速外挂
    /// @author hannibal
    /// @time 2017-10-25
    /// </summary>
    public class SpeedChecker
    {
        // 容忍加速时间（服务器时间）
	    private static int SHC_TOLERANCE_INC	= 5000;
	    // 容忍减速时间（服务器时间）
	    private static int SHC_TOLERANCE_DEC	= 5000;
	    // 检测随机范围 （服务器时间）
	    private static int SHC_MIN_TIME		    = 50*1000;
	    private static int SHC_MAX_TIME		    = 70*1000;
	    // 最大允许发送次数
        private static int MAX_ALLOW_SEND_COUND = 5;

        private long m_session_conn_idx = 0;

        private List<CheckItem> m_send_checks = new List<CheckItem>();
        private uint m_total_times; // 总共检测的次数
        private bool m_need_send;   // 是否需要发送

        public void Reset(long conn_idx)
        {
            m_session_conn_idx = conn_idx;
            m_total_times = 0;
            m_need_send = true;
            m_send_checks.Clear();
        }

        public void Update()
        {
            if (ServerConfig.net_info.speed_hack_check == 0) return;

            // 检测减速以及超时(超时未收到)
            if (Time.frameCount % 8 == 0)// 更新间隔
            {
                if(m_need_send)
                {
                    // 首次或者新的一次
                    SendTrapMsg();
                }
                else if(m_send_checks.Count > 0)
                {
                    int sh_check_timeout_count = 0;
                    long current_time = Time.time;
                    for (int i = 0; i < m_send_checks.Count; ++i)
                    {
                        CheckItem check_item = m_send_checks[i];
                        if (check_item.is_timeout)
                        {
                            sh_check_timeout_count++;
                            continue;
                        }

                        int delta_time = (int)(current_time - (check_item.check_send_time + check_item.check_delay_time));
                        if (delta_time > SHC_TOLERANCE_DEC)
                        {
                            sh_check_timeout_count++;
                            check_item.is_timeout = true;
                        }
                    }

                    if (sh_check_timeout_count >= MAX_ALLOW_SEND_COUND)
                    {
                        this.CloseSession(4);
                        return;
                    }
                    else if (sh_check_timeout_count == m_send_checks.Count && m_send_checks.Count < MAX_ALLOW_SEND_COUND)
                    {
                        // 全部过期 且未满 再次发送
                        m_need_send = true;
                        // 再次发送
                        SendTrapMsg();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 收到检查消息
        /// </summary>
        /// <param name="check_sn"></param>
        public void OnRecvTrapMsg(int check_sn)
        {
            if(m_send_checks.Count == 0)
            {
                this.CloseSession(1);
                return;
            }

            //查询收到的检测项
            bool find = false;
            long find_send_time = 0;
            long current_time = Time.time;
            for (int i = 0; i < m_send_checks.Count; ++i)
            {
                CheckItem check_item = m_send_checks[i];
                if (check_item.sh_check_sn != check_sn)
                    continue;

                // 检测是否加速(提前收到)
                int delta_time = (int)(check_item.check_send_time + check_item.check_delay_time - current_time);
                if(delta_time > SHC_TOLERANCE_INC)
                {
                    this.CloseSession(2);
                    return;
                }

                find = true;
                find_send_time = check_item.check_send_time;
                m_send_checks.RemoveAt(i);// 可以移除已经成功的
                break;
            }

            // 收到错误的数据
            if(!find)
            {
                this.CloseSession(3);
                return;
            }

            // 删除已经过期的
            for (int i = m_send_checks.Count - 1; i >= 0; --i)
            {
                CheckItem check_item = m_send_checks[i];
                if (check_item.check_send_time < find_send_time)
                {
                    m_send_checks.RemoveAt(i);
                }
            }

            // 继续进行下一轮
            if (m_send_checks.Count == 0)
            {
                m_need_send = true;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        private void SendTrapMsg()
        {
            if (m_need_send && m_session_conn_idx > 0)
            {
                //加入队列
                CheckItem item = new CheckItem();
                item.check_delay_time = MathUtils.RandRange(SHC_MIN_TIME, SHC_MAX_TIME);
                item.sh_check_sn = MathUtils.RandRange(0, int.MaxValue);
                item.check_send_time = Time.time;
                m_send_checks.Add(item);

                //发给客户端
                gs2c.SpeedCheck msg = PacketPools.Get(gs2c.msg.SPEED_CHECK) as gs2c.SpeedCheck;
                msg.delay_time = item.check_delay_time;
                msg.check_sn = item.sh_check_sn;
                ClientSession session = ClientSessionManager.Instance.GetSession(m_session_conn_idx);
                if (session != null) session.Send(msg);

                m_need_send = false;
                ++m_total_times;
            }
        }
        /// <summary>
        /// 检测到异常，关闭客户端
        /// </summary>
        private void CloseSession(int id)
        {
            if(m_session_conn_idx > 0)
            {
                Log.Info("检测到使用外挂:" + m_session_conn_idx + " 步骤:" + id);
                ClientSessionManager.Instance.KickoutSession(m_session_conn_idx);
                m_session_conn_idx = 0;
            }
        }
    }

    struct CheckItem
    {
        /// <summary>
        /// 检测消息发送的时间 ms
        /// </summary>
        public long check_send_time;
        /// <summary>
        /// 检测等待时间 ms
        /// </summary>
        public int check_delay_time;
        public int sh_check_sn;
        public bool is_timeout;
    }
}
