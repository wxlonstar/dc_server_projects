using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 协议cd
    /// @author hannibal
    /// @time 2016-9-20
    /// </summary>
    public class StdPacketCDInfo
    {
        public ushort idx;
        public byte type;
        public int cd;
    }

    public class PacketCDConfig : ConfigBase
    {
        static private PacketCDConfig m_Instance;

        private Dictionary<int, StdPacketCDInfo> m_infos = new Dictionary<int, StdPacketCDInfo>();

        static public PacketCDConfig Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = new PacketCDConfig();
                return m_Instance;
            }
        }

        public override bool Load()
        {
            ReadCsvConfig("cd/StdPacketCDInfo.csv", OnLoaded);
            return true;
        }

        public override void Unload()
        {
            m_infos.Clear();
        }

        public override string[] WatcherFiles()
        {
            return new string[1] { "cd/StdPacketCDInfo.csv" };
        }
        private void OnLoaded(CSVDocument doc)
        {
            int totalCount = (int)doc.TableRows();
            for (int i = 0; i < totalCount; ++i)
            {
                StdPacketCDInfo info = new StdPacketCDInfo();
                info.idx = doc.GetValue(i, "idx").ToUInt16();
                info.type = doc.GetValue(i, "type").ToByte();
                info.cd = doc.GetValue(i, "cd").ToInt32();

                if (info.idx > 0)
                {
                    m_infos.Add(info.idx, info);
                }
            }
        }

        public StdPacketCDInfo GetInfo(int id)
        {
            StdPacketCDInfo info;
            if (m_infos.TryGetValue(id, out info))
            {
                return info;
            }
            else
            {
                return null;
            }
        }
    }
}
