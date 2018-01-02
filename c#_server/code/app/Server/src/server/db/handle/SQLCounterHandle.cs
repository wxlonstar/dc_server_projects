using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{
    /// <summary>
    /// 次数
    /// @author hannibal
    /// @time 2016-11-1
    /// </summary>
    public class SQLCounterHandle
    {
        /// <summary>
        /// 修改或插入信息
        /// </summary>
        public static void UpdateCounter(long char_idx, DBID db_id, ByteArray by)
        {
            string sql = "replace into `character_counter`" +
                "(`char_idx`,`bin_use_count`) " +
                "values (" +
                char_idx + "," +
                "@bin_use_count)";

            List<MySqlParameter> param = new List<MySqlParameter>();
            MySqlParameter p = Database.MakeMysqlParam("@bin_use_count", MySqlDbType.Blob, by.GetBuffer(), by.Available);
            param.Add(p);
            DBManager.Instance.GetDB(eDBType.Game, db_id.game_id).Execute(sql, param);
        }
        /// <summary>
        /// 查询信息
        /// </summary>
        public static void QueryCounterList(long char_idx, DBID db_id, Action<bool, ByteArray> callback)
        {
            bool ret = false;
            ByteArray by = DBUtils.AllocDBArray();
            string sql = "call SP_COUNTER_LIST(" + char_idx + ")";
            DBManager.Instance.GetDB(eDBType.Game, db_id.game_id).Query(sql, (reader) =>
            {
                if (reader.HasRows && reader.Read())
                {
                    //内容
                    long len = reader.GetBytes(0, 0, null, 0, int.MaxValue);
                    reader.GetBytes(0, 0, by.Buffer, 0, (int)len);
                    by.WriteEmpty((int)len);
                    ret = true;
                }
                callback(ret, by);
            });
        }
    }
}
