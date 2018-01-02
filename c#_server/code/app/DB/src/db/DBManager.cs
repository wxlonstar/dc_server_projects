using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{    
    /// <summary>
    /// 数据库管理
    /// @author hannibal
    /// @time 2017-8-1
    /// </summary>
    public class DBManager1 : Singleton<DBManager1>
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
            m_db_instance[db_info.id] = db;
        }
        public Database GetDB(eDBType type)
        {
            return m_db_instance[(int)type];
        }
    }
}
