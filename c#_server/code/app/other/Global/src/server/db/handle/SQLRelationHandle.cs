using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{
    /// <summary>
    /// 关系操作
    /// @author hannibal
    /// @time 2016-9-1
    /// </summary>
    public class SQLRelationHandle
    {
        #region 基础关系
        /// <summary>
        /// 查询关系信息
        /// </summary>
        public static void QueryRelationInfo(long char_idx, Action<bool, ByteArray> callback)
        {
            bool ret = false;
            ByteArray by = DBUtils.AllocDBArray();
            string sql = "call SP_RELATION_GET(" + char_idx + ")";
            DBManager.Instance.GetDB(eDBType.Center, 0).Query(sql, (reader) =>
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
        /// <summary>
        /// 修改关系信息
        /// </summary>
        public static void UpdateCharRelation(long char_idx, ByteArray by)
        {
            //TODO:没有关系成员，删除
            if(by.Available == 0)
            {
                string sql = "delete from `relation` where char_idx = " + char_idx + ";";
                DBManager.Instance.GetDB(eDBType.Center, 0).Execute(sql); 
            }
            else
            {
                string sql = "replace into `relation`" +
                "(`char_idx`,`bin_relation`) " +
                "values (" +
                char_idx + "," +
                "@bin_relation)";

                List<MySqlParameter> param = new List<MySqlParameter>();
                MySqlParameter p = Database.MakeMysqlParam("@bin_relation", MySqlDbType.Blob, by.GetBuffer(), by.Available);
                param.Add(p);
                DBManager.Instance.GetDB(eDBType.Center, 0).Execute(sql, param);
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 添加关系事件
        /// </summary>
        public static void InsertRelationEvent(RelationEventInfo info)
        {
            string sql = "call SP_RELATION_EVENT_CREATE(" +
                info.target_char_idx + "," +
                info.source_char_idx + "," +
                (byte)info.event_type + "," +
                "@i_bin_content);";

            ByteArray by = DBUtils.AllocDBArray();
            info.bin_content.Write(by);

            List<MySqlParameter> param = new List<MySqlParameter>();
            MySqlParameter p = Database.MakeMysqlParam("@i_bin_content", MySqlDbType.Blob, by.GetBuffer(), by.Available);
            param.Add(p);
            DBManager.Instance.GetDB(eDBType.Center, 0).Execute(sql, param);
        }
        /// <summary>
        /// 删除关系事件
        /// </summary>
        public static void RemoveRelationEvent(long event_idx)
        {
            string sql = "call SP_RELATION_EVENT_DELETE(" + event_idx + ")";
            DBManager.Instance.GetDB(eDBType.Center, 0).Execute(sql);
        }
        /// <summary>
        /// 查询关系信息
        /// </summary>
        public static void QueryRelationEvent(long char_idx, Action<List<RelationEventInfo>> callback)
        {
            List<RelationEventInfo> list = new List<RelationEventInfo>();
            ByteArray by = DBUtils.AllocDBArray();
            string sql = "call SP_RELATION_EVENT_LOAD(" + char_idx + ")";
            DBManager.Instance.GetDB(eDBType.Center, 0).Query(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RelationEventInfo info = new RelationEventInfo();
                        info.event_idx = reader.GetInt64(0);
                        info.target_char_idx = reader.GetInt64(1);
                        info.source_char_idx = reader.GetInt64(2);
                        info.event_type = (eRelationEvent)reader.GetByte(3);
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
        /// <summary>
        /// 查询是否包含关系事件
        /// </summary>
        public static void QueryExistsRelationEvent(long char_idx, long target_char_idx, eRelationFlag flag, Action<long> callback)
        {
            string sql = "call SP_RELATION_EVENT_CHECK(" + target_char_idx + "," + char_idx + "," + (byte)flag + ")";
            DBManager.Instance.GetDB(eDBType.Center, 0).Query(sql, (reader) =>
            {
                long event_idx = 0;
                if (reader.HasRows && reader.Read())
                {
                    event_idx = reader.GetInt64(0);
                }
                callback(event_idx);
            });
        }
        #endregion
    }
}
