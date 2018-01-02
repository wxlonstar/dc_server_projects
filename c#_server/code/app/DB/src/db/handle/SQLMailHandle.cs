using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{
    /// <summary>
    /// 邮件操作
    /// @author hannibal
    /// @time 2017-9-1
    /// </summary>
    public class SQLMailHandle
    {
        private static ByteArray by_array = new ByteArray(1024, 65535);

        /// <summary>
        /// 加载玩家已经领取过的邮件列表
        /// </summary>
        /// <param name="char_idx">收件人</param>
        /// <param name="callback"></param>
        public static void LoadCharRecvs(long char_idx, Action<MailCharRecvs> callback)
        {
            string sql = "call SP_MAIL_GET_CHAR_RECV(" + char_idx + ")";
            DBManager.Instance.GetDB(eDBType.Game).Query(sql, (reader) =>
            {
                MailCharRecvs info = new MailCharRecvs();
                if (reader.HasRows && reader.Read())
                {
                    long len = reader.GetBytes(0, 0, null, 0, int.MaxValue);
                    if(len > 0)
                    {
                        ByteArray by = DBUtils.AllocDBArray();
                        reader.GetBytes(0, 0, by.Buffer, 0, (int)len);
                        by.WriteEmpty((int)len);//直接修改m_tail
                        info.Read(by);
                    }
                }
                callback(info);
            });
        }
        /// <summary>
        /// 修改已经领取过的邮件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public static void UpdateCharRecvs(long char_idx, MailCharRecvs info)
        {
            string sql = "replace into `mail_char_recv`" +
            "(`char_idx`,`bin_mails`) " +
            "values (" +
            char_idx + "," +
            "@bin_mails)";

            ByteArray by = DBUtils.AllocDBArray();
            info.Write(by);
            List<MySqlParameter> param = new List<MySqlParameter>();
            MySqlParameter p = Database.MakeMysqlParam("@bin_mails", MySqlDbType.Blob, by.GetBuffer(), by.Available);
            param.Add(p);
            DBManager.Instance.GetDB(eDBType.Game).Execute(sql, param);
        }
        /// <summary>
        /// 加载邮件列表
        /// </summary>
        /// <param name="receiver_idx">收件人</param>
        /// <param name="callback"></param>
        public static void LoadMailList(long receiver_idx, ushort spid, Action<List<MailInfo>> callback)
        {
            string sql = "call SP_MAIL_LIST(" + receiver_idx + ", " + spid + ", " + Time.second_time + ")";
            DBManager.Instance.GetDB(eDBType.Game).Query(sql, (reader) =>
            {
                List<MailInfo> list = new List<MailInfo>();
                while (reader.HasRows && reader.Read())
                {
                    int idx = 0;
                    MailInfo data = CommonObjectPools.Spawn<MailInfo>();
                    data.mail_idx = reader.GetInt64(idx++);
                    data.mail_type = (eMailType)reader.GetByte(idx++);
                    data.spid = reader.GetUInt16(idx++);
                    data.sender_idx = reader.GetInt64(idx++);
                    data.sender_name = reader.GetString(idx++);
                    data.send_time = reader.GetInt64(idx++);
                    data.expire_time = reader.GetInt32(idx++);
                    data.flags = reader.GetUInt32(idx++);
                    data.subject = reader.GetString(idx++);
                    //内容
                    long len = reader.GetBytes(idx, 0, null, 0, int.MaxValue);
                    ByteArray by = DBUtils.AllocDBArray();
                    reader.GetBytes(idx, 0, by.Buffer, 0, (int)len);
                    by.WriteEmpty((int)len);//直接修改m_tail
                    data.bin_mail_content.Read(by);
                    list.Add(data);
                }
                callback(list);
            });
        }
        /// <summary>
        /// 创建邮件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public static void CreateMail(MailInfo info)
        {
            string sql = "insert into `mail_box` " +
            "(mail_type,spid,receiver_idx,sender_idx,sender_name,send_time,expire_time,delivery_time,flags,`subject`,bin_mail_content,last_update_time) " +
            "values (" +
            (byte)info.mail_type + "," +
            info.spid + "," +
            info.receiver_idx + "," +
            info.sender_idx + ",'" +
            info.sender_name + "'," +
            info.send_time + "," +
            info.expire_time + "," +
            info.delivery_time + "," +
            info.flags + ",'" +
            info.subject + "'," +
            "@bin_content,now())";

            ByteArray by = DBUtils.AllocDBArray();
            info.bin_mail_content.Write(by);
            List<MySqlParameter> param = new List<MySqlParameter>();
            MySqlParameter p = Database.MakeMysqlParam("@bin_content", MySqlDbType.Blob, by.GetBuffer(), by.Available);
            param.Add(p);
            DBManager.Instance.GetDB(eDBType.Game).Execute(sql, param);
        }
        /// <summary>
        /// 删除邮件：批量删除
        /// </summary>
        /// <param name="mail_idx"></param>
        public static void DeleteMail(List<long> list)
        {
            while(list.Count > 0)
            {
                int del_count = 0;
                string sql = "DELETE FROM mail_box WHERE mail_box.mail_idx in( ";
                for (int i = list.Count - 1; i >= 0; i--, del_count++)
                {
                    if (del_count != 0) sql += ",";
                    sql += list[i];
                    list.RemoveAt(i);
                    if (del_count >= 100) break;
                }
                sql += ");";
                if (del_count > 0) DBManager.Instance.GetDB(eDBType.Game).Execute(sql);
            }
        }
        /// <summary>
        /// 修改邮件标记：支持同时修改多行
        /// </summary>
        /// <param name="flags"></param>
        public static void ModifyMailFlags(Dictionary<long, MailInfo> all_mails, List<long> list)
        {
            while (list.Count > 0)
            {
                int update_count = 0;
                string sql = "UPDATE mail_box SET flags = CASE mail_idx ";
                string sql_end = "";
                for (int i = list.Count - 1; i >= 0; i--, update_count++)
                {
                    long mail_idx = list[i];
                    MailInfo mail_info;
                    if(all_mails.TryGetValue(mail_idx, out mail_info))
                    {
                        sql = sql + " WHEN " + mail_idx + " THEN " + mail_info.flags;
                        if (update_count != 0) sql_end += ", ";
                        sql_end += mail_idx.ToString();
                        list.RemoveAt(i);
                        if (update_count >= 10) break;
                    }
                }
                sql = sql + " END WHERE mail_idx in(" + sql_end + ");";
                if (update_count > 0) DBManager.Instance.GetDB(eDBType.Game).Execute(sql);
            }
        }
    }
}
