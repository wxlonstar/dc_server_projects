using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace dc
{
    /// <summary>
    /// token对象池
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public class UserTokenPools
    {
        private static int m_total_new_count = 0;
        private static int m_total_remove_count = 0;
        private ConcurrentBag<UserToken> m_pools = new ConcurrentBag<UserToken>();

        public UserToken Spawn()
        {
            UserToken obj = null;
            if (!m_pools.TryTake(out obj))
            {
                System.Threading.Interlocked.Increment(ref m_total_new_count);
                obj = new UserToken();
            }
            else
            {
                System.Threading.Interlocked.Decrement(ref m_total_remove_count);
            }
            return obj;
        }
        public void Despawn(UserToken obj)
        {
            if (obj == null) return;
            obj.Reset();
            m_pools.Add(obj);
            System.Threading.Interlocked.Increment(ref m_total_remove_count);
        }
        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("UserTokenPools使用情况:");
            st.AppendLine("New次数:" + m_total_new_count + " 空闲数量:" + m_total_remove_count);
            if (is_print) Console.WriteLine(st.ToString());
            return st.ToString();
        }
    }
}
