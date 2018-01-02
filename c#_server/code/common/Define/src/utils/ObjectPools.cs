using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace dc
{

    /// <summary>
    /// 对象池
    /// 1.使用对象池，需要把原有对象的集合清空(list,dictionary等)
    /// @author hannibal
    /// @time 2016-7-24
    /// </summary>
    public sealed class ObjectPools
    {
        private int m_total_new_count = 0;
        //一个无序的集合，程序可以向其中插入元素，或删除元素。在同一个线程中向集合插入，删除元素的效率很高。
        private ConcurrentBag<object> m_obj_pools = new ConcurrentBag<object>();
        private static ConcurrentDictionary<string, long> m_new_count = new ConcurrentDictionary<string, long>();
        private static ConcurrentDictionary<string, long> m_remove_count = new ConcurrentDictionary<string, long>();

        public T Spawn<T>() where T : new()
        {
            object obj = null;
            if (!m_obj_pools.TryTake(out obj))
            {
                System.Threading.Interlocked.Increment(ref m_total_new_count);
                //Log.Debug("总共分配的object:" + m_total_new_count);
                obj = new T();

                //修改次数
                Type t = typeof(T);
                long count = 0;
                if (!m_new_count.TryGetValue(t.FullName, out count))
                    m_new_count.TryAdd(t.FullName, 1);
                else
                    m_new_count[t.FullName] = System.Threading.Interlocked.Increment(ref count);
            }
            //初始化
            if (obj is IPoolsObject)
            {
                ((IPoolsObject)obj).Init();
            }
            return (T)obj;
        }
        public void Despawn<T>(T obj) where T : new()
        {
            if (obj == null) return;
            m_obj_pools.Add(obj);

            //修改次数
            Type t = typeof(T);
            long count = 0;
            if (!m_remove_count.TryGetValue(t.FullName, out count))
                m_remove_count.TryAdd(t.FullName, 1);
            else
                m_remove_count[t.FullName] = System.Threading.Interlocked.Increment(ref count);
        }
        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("ObjectPools使用情况:");
            foreach (var obj in m_new_count)
            {
                string class_name = obj.Key;
                string one_line = class_name + " New次数:" + obj.Value;
                long count;
                if (m_remove_count.TryGetValue(class_name, out count))
                {
                    one_line += " 空闲数量:" + count;
                }
                st.AppendLine(one_line);
            }
            if (is_print) Console.WriteLine(st.ToString());
            return st.ToString();
        }
    }

    public sealed class CommonObjectPools
    {
        private static ConcurrentDictionary<string, ConcurrentBag<object>> m_pools = new ConcurrentDictionary<string, ConcurrentBag<object>>();
        private static ConcurrentDictionary<string, long> m_new_count = new ConcurrentDictionary<string, long>();
        /// <summary>
        /// 分配对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">类名</param>
        /// <returns></returns>
        public static T Spawn<T>() where T : new()
        {
            object obj = null;
            Type t = typeof(T);
            ConcurrentBag<object> list;
            if(m_pools.TryGetValue(t.FullName, out list))
            {
                object _obj = null;
                if (list.TryTake(out _obj) && _obj is T)
                    obj = _obj;
            }
            //创建新对象
            if(obj == null)
            {
                obj = new T();
                //修改次数
                long count = 0;
                if (!m_new_count.TryGetValue(t.FullName, out count))
                    m_new_count.TryAdd(t.FullName, 1);
                else
                    m_new_count[t.FullName] = System.Threading.Interlocked.Increment(ref count);
            }
            //初始化
            if(obj is IPoolsObject)
            {
                ((IPoolsObject)obj).Init();
            }
            return (T)obj;
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        public static void Despawn(object obj)
        {
            if (obj == null) return;
            Type t = obj.GetType();
            string name = t.FullName;
            ConcurrentBag<object> list;
            if (!m_pools.TryGetValue(name, out list))
            {
                list = new ConcurrentBag<object>();
                m_pools.TryAdd(name, list);
            }
            list.Add(obj);
        }

        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("CommonObjectPools使用情况:");
            foreach(var obj in m_new_count)
            {
                string class_name = obj.Key;
                string one_line = class_name + " New次数:" + obj.Value;
                ConcurrentBag<object> list;
                if (m_pools.TryGetValue(class_name, out list))
                {
                    one_line += " 空闲数量:" + list.Count;
                }
                st.AppendLine(one_line);
            }
            if (is_print) Console.WriteLine(st.ToString());
            return st.ToString();
        }
    }
}
