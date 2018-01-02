using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{
    /// <summary>
    /// 角色操作
    /// @author hannibal
    /// @time 2016-9-1
    /// </summary>
    public class SQLCharHandle
    {
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～game～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        /// <summary>
        /// 最大角色id
        /// </summary>
        /// <param name="callback"></param>
        public static void QueryMaxCharIdx(ushort ws_id, DBID db_id, Action<long> callback)
        {
            string sql = "select count(*), max(char_index) from `character` where ws_id = " + ws_id;
            long max_id = 0;//从这个开始
            DBManager.Instance.GetDB(eDBType.Game, db_id.game_id).Query(sql, (reader) =>
            {
                if (reader.HasRows && reader.Read())
                {
                    int count = reader.GetInt32(0);
                    if (count > 0) max_id = reader.GetInt64(1);
                }
                callback(max_id);
            });
        }

        /// <summary>
        /// 创号
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public static void CreateCharacter(long account_idx, DBID db_id, CreateCharacterInfo info, Action<long> callback)
        {
            string sql = "call SP_CHARACTER_CREATE("
                + 1 + ","
                + info.char_idx + ","
                + account_idx + ","
                + info.spid + ",'"
                + info.char_name + "',"
                + info.char_type + ","
                + info.ws_id + ","
                + info.ss_id + ","
                + info.fs_id
                + ")";
            DBManager.Instance.GetDB(eDBType.Game, db_id.game_id).Query(sql, (reader) =>
            {
                long result = 0;
                if (reader.HasRows && reader.Read())
                {
                    result = reader.GetInt64(0);
                }
                callback(result);
            });
        }
    }
}
