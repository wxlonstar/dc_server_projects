using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{
    /// <summary>
    /// db事件
    /// @author hannibal
    /// @time 2016-11-1
    /// </summary>
    public class SQLDBEventHandle
    {
        /// <summary>
        /// 添加事件
        /// </summary>
        public static void InsertDBEvent(DBEventInfo info, DBID db_id)
        {
            string sql = "call SP_DB_EVENT_CREATE(" +
                info.target_char_idx + "," +
                info.source_char_idx + "," +
                (byte)info.event_type + "," +
                "@i_bin_content);";

            ByteArray by = DBUtils.AllocDBArray();
            info.bin_content.Write(by);

            List<MySqlParameter> param = new List<MySqlParameter>();
            MySqlParameter p = Database.MakeMysqlParam("@i_bin_content", MySqlDbType.Blob, by.GetBuffer(), by.Available);
            param.Add(p);
            DBManager.Instance.GetDB(eDBType.Game, db_id.game_id).Execute(sql, param);
        }
        /// <summary>
        /// 批量删除事件
        /// </summary>
        public static void DeleteDBEvent(List<long> list, DBID db_id)
        {
            while (list.Count > 0)
            {
                int del_count = 0;
                string sql = "DELETE FROM game_event WHERE game_event.event_idx in( ";
                for (int i = list.Count - 1; i >= 0; i--, del_count++)
                {
                    if (del_count != 0) sql += ",";
                    sql += list[i];
                    list.RemoveAt(i);
                    if (del_count >= 100) break;
                }
                sql += ");";
                if (del_count > 0) DBManager.Instance.GetDB(eDBType.Game, db_id.game_id).Execute(sql);
            }
        }
        /// <summary>
        /// 查询信息
        /// </summary>
        public static void QueryDBEvent(long char_idx, DBID db_id, Action<List<DBEventInfo>> callback)
        {
            List<DBEventInfo> list = new List<DBEventInfo>();
            ByteArray by = DBUtils.AllocDBArray();
            string sql = "call SP_DB_EVENT_LOAD(" + char_idx + ")";
            DBManager.Instance.GetDB(eDBType.Game, db_id.game_id).Query(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        DBEventInfo info = new DBEventInfo();
                        info.event_idx = reader.GetInt64(0);
                        info.target_char_idx = reader.GetInt64(1);
                        info.source_char_idx = reader.GetInt64(2);
                        info.event_type = (eDBEventType)reader.GetByte(3);
                        //内容
                        by.Clear();
                        long len = reader.GetBytes(4, 0, null, 0, int.MaxValue);
                        reader.GetBytes(4, 0, by.Buffer, 0, (int)len);
                        by.WriteEmpty((int)len);
                        info.bin_content.Read(by);

                        list.Add(info);
                    }
                }
                callback(list);
            });
        }
    }
}
