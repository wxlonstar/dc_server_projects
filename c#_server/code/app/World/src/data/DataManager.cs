﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 数据管理器
    /// @author hannibal
    /// @time 2016-8-1
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        public void Setup()
        {
            ConfigManager.Instance.Setup();
            ServerInfoManager.Instance.Setup();
        }
        public void Destroy()
        {
            ConfigManager.Instance.Destroy();
            ServerInfoManager.Instance.Destroy();
        }
        public void Tick()
        {
        }
        public void LoadAll()
        {
            ConfigManager.Instance.LoadAll();
        }
    }
}
