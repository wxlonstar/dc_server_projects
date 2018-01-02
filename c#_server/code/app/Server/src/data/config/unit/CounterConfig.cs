using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 次数
    /// @author hannibal
    /// @time 2016-11-2
    /// </summary>
    public class StdCounterInfo
    {
        public ushort  idx;
        public ushort value;
        public bool reset;
        public ushort cd_type;
        public Dictionary<int, int> cd_value = null;
    }

    public class CounterConfig : ConfigBase
    {
        static private CounterConfig m_Instance;

        private Dictionary<int, StdCounterInfo> m_infos = new Dictionary<int, StdCounterInfo>();

        static public CounterConfig Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = new CounterConfig();
                return m_Instance;
            }
        }

        public override bool Load()
        {
            ReadCsvConfig("counter/StdCounterInfo.csv", OnLoad);
            return true;
        }

        public override void Unload()
        {
            m_infos.Clear();
        }

        public override string[] WatcherFiles()
        {
            return new string[1] { "counter/StdCounterInfo.csv" };
        }
        private void OnLoad(CSVDocument doc)
        {
            int totalCount = (int)doc.TableRows();
            for (int i = 0; i < totalCount; ++i)
            {
                StdCounterInfo info = new StdCounterInfo();
                info.idx = doc.GetValue(i, "idx").ToUInt16();
                info.value = doc.GetValue(i, "value").ToUInt16();
                info.reset = doc.GetValue(i, "reset").ToBool();
                info.cd_type = doc.GetValue(i, "cd_type").ToUInt16();
                string cd = doc.GetValue(i, "cd_value").ToString();
                switch(info.cd_type)
                {
                    case 2:
                        info.cd_value = new Dictionary<int,int>();
                        info.cd_value.Add(0, int.Parse(cd));
                        break;

                    case 3:
                        info.cd_value = new Dictionary<int, int>();
                        
                        string[] arr_cds = cd.Split(',');
                        if (arr_cds.Length > 0)
                        {
                            for (int j = 0; j < arr_cds.Length; ++j)
                            {
                                string[] arr_cd = arr_cds[j].Split('|');
                                info.cd_value.Add(int.Parse(arr_cd[0]), int.Parse(arr_cd[1]));
                            }
                        }
                        break;
                }

                if (info.idx > 0)
                    m_infos.Add(info.idx, info);
            }
        }

        public StdCounterInfo GetInfo(int id)
        {
            StdCounterInfo info;
            if (m_infos.TryGetValue(id, out info))
            {
                return info;
            }
            else
            {
                Log.Warning(string.Format("Not find Counterinfo id = {0}", id));
                return null;
            }
        }

        public Dictionary<int, StdCounterInfo> infos
        {
            get { return m_infos; }
        }
    }
}
