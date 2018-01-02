using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace dc
{
    /// <summary>
    /// 配置表读取器
    /// @author hannibal
    /// @time 2016-9-10
    /// </summary>
    public abstract class ConfigBase
    {
        protected string m_config_path;

        /// <summary>
        /// 加载
        /// </summary>
        public abstract bool Load();
        /// <summary>
        /// 释放
        /// </summary>
        public abstract void Unload();
        /// <summary>
        /// 监视文件列表，需要包括扩展名和路径，文件不区分大小写
        /// </summary>
        /// <returns></returns>
        public abstract string[] WatcherFiles();

        /// <summary>
        /// 读txt文件
        /// </summary>
        public virtual bool ReadTxtConfig(string fileName, Action<string[]> handler)
        {
            Log.Info("ReadTxtConfig:" + fileName);
            try
            {
                string file_data = File.ReadAllText(m_config_path + fileName);
                string sText = file_data.Replace("\n", "").Replace("\\n", "\n");
                handler(sText.Split('\r'));
                return true;
            }
            catch(Exception e)
            {
                Log.Exception(e);
            }
            return false;
        }

        /// <summary>
        /// 读取CSV配置
        /// </summary>
        public virtual bool ReadCsvConfig(string fileName, Action<CSVDocument> handler)
        {
            Log.Info("ReadCsvConfig:" + fileName);
            CSVDocument csvDocument = new CSVDocument();
            if (csvDocument.Load(m_config_path + fileName))
            {
                handler(csvDocument);
            }
            csvDocument.Clear();
            csvDocument = null;
            return true;
        }

        /// <summary>
        /// 读取Xml配置
        /// </summary>
        public virtual bool ReadXmlConfig(string fileName, Action<XmlDocument> handler)
        {
            Log.Info("ReadXmlConfig:" + fileName);
            try
            {
                string file_data = File.ReadAllText(m_config_path + fileName);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(file_data);
                handler(xmlDoc);
                xmlDoc = null;
                return true;
            }
            catch(Exception e)
            {
                Log.Exception(e);
            }
            return false;
        }
        /// <summary>
        /// 设置读取目录
        /// </summary>
        public void SetConfigPath(string pathFile)
        {
            m_config_path = pathFile;
        }
    }
}
