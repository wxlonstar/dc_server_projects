using System;
using System.Collections.Generic;
using System.IO;

namespace dc
{
    /// <summary>
    /// 配置表管理
    /// @author hannibal
    /// @time 2016-8-1
    /// </summary>
    public class ConfigManager : Singleton<ConfigManager>
    {        
        //监视资源目录
        private FileDirWatcher m_config_watcher = null;
        //所有配置表
        private List<ConfigBase> m_configs = new List<ConfigBase>();
        private Dictionary<string, ConfigBase> m_watch_configs = new Dictionary<string, ConfigBase>();

        public void Setup()
        {
            m_config_watcher = new FileDirWatcher();
            m_config_watcher.Start(@"..\data\config", "*.csv", true);
            m_config_watcher.OnChangedEvt += OnConfigChanged;

            AddConfig(PacketCDConfig.Instance); 
        }
        public void Destroy()
        {
            if (m_config_watcher != null)
            {
                m_config_watcher.Stop();
                m_config_watcher = null;
            }

            UnloadAll();
            m_configs.Clear();
            m_watch_configs.Clear();
        }
        public void LoadAll()
        {
            foreach (var obj in m_configs)
            {
                obj.Load();
            }
        }
        public void UnloadAll()
        {
            foreach (var obj in m_configs)
            {
                obj.Unload();
            }
        }
        private void AddConfig(ConfigBase config, string pathFile = "../data/config/")
        {
            config.SetConfigPath(pathFile);
            m_configs.Add(config);
            //监视文件
            string[] arrs = config.WatcherFiles();
            if(arrs != null && arrs.Length > 0)
            {
                foreach(var file in arrs)
                {
                    string file_name = file.ToLower();
                    file_name = file_name.Replace("\\", "/");
                    if (m_watch_configs.ContainsKey(file_name))
                    {
                        Log.Error("文件已经在监视中:" + file_name);
                        continue;
                    }
                    m_watch_configs.Add(file_name, config);
                }
            }
        }
        /// <summary>
        /// 文件内容改变
        /// </summary>
        /// <param name="file_path">改变文件全路径</param>
        private void OnConfigChanged(string file_path)
        {
            file_path = file_path.Replace("\\", "/");
            file_path = file_path.ToLower();
            foreach (var obj in m_watch_configs)
            {
                string file_name = obj.Key;
                if (file_path.Contains(file_name))
                {
                    ConfigBase config = obj.Value;
                    config.Unload();
                    config.Load();
                }
            }
        }
    }
}
