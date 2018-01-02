using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{    
    /// <summary>
    /// 数据库管理
    /// @author hannibal
    /// @time 2016-8-1
    /// </summary>
    public class DatabaseManager : Singleton<DatabaseManager>
    {
        private Database[] m_db_instance = new Database[(int)eDBType.Max];

        public void Setup()
        {
            
        }
        public void Destroy()
        {
            for (int i = 0; i < (int)eDBType.Max; ++i)
            {
                if(m_db_instance[i] != null)
                {
                    m_db_instance[i].Close();
                    m_db_instance[i] = null;
                }
            }
        }
        public void Tick()
        {

        }
        public void Start()
        {
            //打开db连接
            foreach (var db_info in ServerConfig.info.db_list)
            {
                OpenDB(db_info);
            }
        }
        private void OpenDB(ServerNetInfo.DBItems db_info)
        {
            string conn_str = "Database='" + db_info.name + "';Data Source='" + db_info.address + "';User Id='" + db_info.username + "';Password='" + db_info.password + "';charset='utf8';pooling=true";
            Database db = new Database();
            db.Open(conn_str, db_info.name);
            m_db_instance[db_info.id] = db;
        }
        public Database GetDB(eDBType type) 
        {
            return m_db_instance[(int)type];
        }
        /// <summary>
        /// 获取数据库表列表
        /// </summary>
        public System.Data.DataTable GetTables(eDBType type)
        {
            Database db = GetDB(type);
            if (db == null) return null;

            return db.GetTables();
        }
        /// <summary>
        /// 获取表字段列表
        /// </summary>
        public Dictionary<string, Type> GetFields(eDBType type, string table_name)
        {
            Database db = GetDB(type);
            if (db == null) return null;

            Dictionary<string, Type> ret_list = null;
            db.GetFields(table_name, (list) => { ret_list = list; });

            return ret_list;
        }
    }
}
