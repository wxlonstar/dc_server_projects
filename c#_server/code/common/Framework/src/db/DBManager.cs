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
    public class DBManager : Singleton<DBManager>
    {
        private Dictionary<string, Database> m_db_instance = null;

        public DBManager()
        {
            m_db_instance = new Dictionary<string, Database>();
        }
        public void Setup()
        {
            
        }
        public void Destroy()
        {
            foreach(var db in m_db_instance)
            {
                db.Value.Close();
            }
            m_db_instance.Clear();
        }
        public void Tick()
        {

        }
        public void Start(List<DBItems> list)
        {
            //打开db连接
            foreach (var db_info in list)
            {
                OpenDB(db_info);
            }
        }
        private void OpenDB(DBItems db_info)
        {
            string conn_str = "Database='" + db_info.name + "';Data Source='" + db_info.address + "';User Id='" + db_info.username + "';Password='" + db_info.password + "';charset='utf8';pooling=true";
            Database db = new Database();
            db.Open(conn_str, db_info.name);
            m_db_instance.Add((int)db_info.type + "_" + db_info.id, db);
        }
        public Database GetDB(eDBType type, ushort id) 
        {
            Database db;
            if (m_db_instance.TryGetValue((int)type + "_" + id, out db))
                return db;
            return null;
        }
    }
}
