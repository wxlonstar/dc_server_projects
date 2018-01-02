using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace dc
{
    /// <summary>
    /// 协议对象池
    /// @author hannibal
    /// @time 2016-8-12
    /// </summary>
    public class PacketPools
    {
        private static Dictionary<ushort, string> m_packet_factory = new Dictionary<ushort, string>();
        private static Dictionary<string, List<PacketBase>> m_packets = new Dictionary<string, List<PacketBase>>();
        private static Dictionary<string, long> m_new_count = new Dictionary<string, long>();

        public static void Register(ushort id, string name)
        {
            if(m_packet_factory.ContainsKey(id))return;
            m_packet_factory.Add(id, "dc." + name);
        }

        public static PacketBase Get(ushort id)
        {
            string name = "";
            if (!m_packet_factory.TryGetValue(id, out name))
            {
                Console.WriteLine("未注册协议id:" + id);
                return null;
            }
            PacketBase packet = null;
            List<PacketBase> list;
            if (m_packets.TryGetValue(name, out list) && list.Count > 0)
            {
                packet = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
            }
            else
            {
                Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
                object obj = assembly.CreateInstance(name);
                packet = obj as PacketBase;

                //统计分配次数
                long count = 0;
                if (!m_new_count.TryGetValue(name, out count))
                    m_new_count.Add(name, 1);
                else
                    m_new_count[name] = ++count;
            }
            packet.Init();
            packet.header = id;
            return packet;
        }
        public static void Recover(PacketBase obj)
        {
            string name = obj.GetType().FullName;
            List<PacketBase> list;
            if (!m_packets.TryGetValue(name, out list))
            {
                list = new List<PacketBase>();
                m_packets.Add(name, list);
            }
            if (!list.Contains(obj)) list.Add(obj);
        }

        public static string ToString(bool is_print)
        {
            StringBuilder st = new StringBuilder();
            st.AppendLine("PacketPools使用情况:");
            foreach (var obj in m_new_count)
            {
                string class_name = obj.Key;
                string one_line = class_name + " New次数:" + obj.Value;
                List<PacketBase> list;
                if (m_packets.TryGetValue(class_name, out list))
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
