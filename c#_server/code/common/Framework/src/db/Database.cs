using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace dc
{
    /// <summary>
    /// 数据库
    /// @author hannibal
    /// @time 2016-8-1
    /// </summary>
    public sealed class Database
    {
        private string m_DBName = "";
        private MySqlConnection m_MysqlConn = null;

        private bool m_IsExecing = false;
        /**执行队列，异步查询*/
        private Queue<sSQLExecInfo> m_SQLExecQueue = null;

        public Database()
        {
            m_SQLExecQueue = new Queue<sSQLExecInfo>();
        }

        public bool Open(string conn, string db_name)
        {
            if (conn.Length == 0 || db_name.Length == 0) return false;
            if (IsOpen()) return true;
            m_IsExecing = false;

            try
            {
                m_DBName = db_name;
                m_MysqlConn = new MySqlConnection(conn);
                m_MysqlConn.Open();
                Log.Info("数据库打开成功:" + conn);
            }
            catch(Exception e)
            {
                m_MysqlConn.Dispose();
                m_MysqlConn = null;
                Log.Exception(e);
                return false;
            }
            return true;
        }

        public void Close()
        {
            if(m_MysqlConn != null)
            {
                m_MysqlConn.Close();
                m_MysqlConn.Dispose();
                m_MysqlConn = null;
            }
            m_SQLExecQueue.Clear();
        }
        /// <summary>
        /// 查询操作，异步执行
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="callback">执行完回调</param>
        public void Query(string sql, Action<MySqlDataReader> callback)
        {
            if (!IsOpen())
            {
                Log.Warning("连接已经关闭，执行语句:" + sql);
                return;
            }
            if (m_IsExecing)
            {
                m_SQLExecQueue.Enqueue(new sSQLExecInfo(eSQLExecType.QUERY, sql, null, callback));
                return;
            }
            //Log.Debug("[db]:query async:" + sql);
            m_IsExecing = true;
            Task<MySqlDataReader> task = MySqlHelper.ExecuteReaderAsync(m_MysqlConn, sql, null);
            task.ContinueWith((t) =>
            {
                lock (ThreadScheduler.Instance.LogicLock)
                {
                    if(t.Exception != null)
                    {
                        Log.Exception(t.Exception.InnerException);
                    }
                    else if (t.Status != TaskStatus.RanToCompletion || t.Result == null)
                    {
                        Log.Error("未知错误，执行sql:" + sql);
                    }
                    else
                    {
                        try
                        {
                            if (callback != null) callback(t.Result);
                        }
                        catch(Exception e)
                        {
                            Log.Exception(e);
                        }
                        task.Result.Close();
                        task.Result.Dispose();
                    }
                    m_IsExecing = false;
                    CheckSqlExec();
                }
            });
        }
        /// <summary>
        /// 查询操作：同步执行
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="callback"></param>
        public void QuerySync(string sql, Action<MySqlDataReader> callback)
        {
            if (!IsOpen())
            {
                Log.Warning("连接已经关闭，执行语句:" + sql);
                return;
            }
            //Log.Debug("[db]:QuerySync:" + sql);
            try
            {
                using (MySqlDataReader reader = MySqlHelper.ExecuteReader(m_MysqlConn, sql, null))
                {
                    if (callback != null) callback(reader);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
        /// <summary>
        /// 添加，删除，插入等操作；同步执行
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>影响的行数</returns>
        public void Execute(string sql, List<MySqlParameter> param = null)
        {
            if (!IsOpen())
            {
                Log.Warning("连接已经关闭，执行语句:" + sql);
                return;
            }
            if (m_IsExecing)
            {
                m_SQLExecQueue.Enqueue(new sSQLExecInfo(eSQLExecType.Exec, sql, param, null));
                return;
            }
            //Log.Debug("[db]:execute:" + sql);
            try
            {
                if (param != null)
                {
                    MySqlParameter[] arr_p = param.ToArray();
                    MySqlHelper.ExecuteNonQuery(m_MysqlConn, sql, arr_p);
                    param.ForEach((p) => { CommonObjectPools.Despawn(p); });
                }
                else
                {
                    MySqlHelper.ExecuteNonQuery(m_MysqlConn, sql, null);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
            CheckSqlExec();
        }
        /// <summary>
        /// 是否执行完成一条语句
        /// </summary>
        private void CheckSqlExec()
        {
            if (m_SQLExecQueue.Count == 0 || m_IsExecing) return;

            sSQLExecInfo info = m_SQLExecQueue.Dequeue();
            switch (info.type)
            {
                case eSQLExecType.Exec:
                    Execute(info.sql, info.param);
                    break;

                case eSQLExecType.QUERY:
                    Query(info.sql, info.callback);
                    break;
            }
        }

        public bool IsOpen()
        {
            if (m_MysqlConn == null || m_MysqlConn.State == ConnectionState.Closed || m_MysqlConn.State == ConnectionState.Broken)
                return false;

            return true;
        }
        /// <summary>
        /// 获取数据库表列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTables()
        {
            if (!IsOpen()) return null;

            //--将表名保存到datatable中  
            DataTable dt = m_MysqlConn.GetSchema("Tables", null);
            return dt;
        }
        /// <summary>
        /// 获取表字段列表
        /// </summary>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public void GetFields(string table_name, Action<Dictionary<string, Type>> callback)
        {
            Dictionary<string, Type> list = new Dictionary<string, Type>();

            string sql = "show columns from `" + table_name + "`;";
            MySqlDataReader reader = MySqlHelper.ExecuteReader(m_MysqlConn, sql, null);
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    string n = reader.GetString(0);
                    object obj = reader.GetValue(1);
                    Type t = obj as Type;
                    list.Add(n, t);
                }
                callback(list);
                reader.Dispose();
                reader.Close();
            }
            else
            {

                callback(list);
            }
        }
        /// <summary>
        /// 构建mysql参数
        /// </summary>
        /// <param name="filed">字段</param>
        /// <param name="type">字段类型</param>
        /// <param name="by">内容</param>
        /// <param name="size">内容大小</param>
        /// <returns></returns>
        public static MySqlParameter MakeMysqlParam(string filed, MySqlDbType type, byte[] by, int size)
        {
            MySqlParameter p = CommonObjectPools.Spawn<MySqlParameter>();
            p.ParameterName = filed;
            p.MySqlDbType = type;
            p.Value = by;
            p.Size = size;
            return p;
        }
    }
    /// <summary>
    /// sql执行数据
    /// </summary>
    struct sSQLExecInfo
    {
        public eSQLExecType type;
        public string sql;
        public List<MySqlParameter> param;
        public Action<MySqlDataReader> callback;

        public sSQLExecInfo(eSQLExecType _type, string _sql, List<MySqlParameter> _param, Action<MySqlDataReader> _fun)
        {
            type = _type;
            sql = _sql;
            param = _param;
            callback = _fun;
        }
    }
    /// <summary>
    /// 执行类型
    /// </summary>
    enum eSQLExecType
    {
        QUERY,
        Exec,
    }
}
