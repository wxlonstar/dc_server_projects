using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{
    /// <summary>
    /// 数据库操作
    /// @author hannibal
    /// @time 2016-8-1
    /// </summary>
    public class SQLLoginHandle
    {
        /// <summary>
        /// 账号数据：登录账号验证用
        /// </summary>
        /// <param name="username">登录用户名</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void QueryAccountData(string username, Action<AccountData> callback)
        {
            string sql = "call SP_GET_ACCOUNT_LOGINDATA('" + username + "')";
            DBManager.Instance.GetDB(eDBType.Member, 0).Query(sql, (reader) =>
            {
                AccountData data = new AccountData();
                if (reader.HasRows && reader.Read())
                {
                    data.account_idx = reader.GetInt64(0);
                    data.password_md5 = reader.GetString(1);
                    data.account_type = reader.GetByte(2);
                    data.spid = reader.GetUInt16(3);
                    callback(data);
                }
                else
                {
                    callback(data);
                }
            });
        }
        /// <summary>
        /// 登录：成功会更新login_status表
        /// </summary>
        /// <param name="account_idx">账号id</param>
        /// <param name="callback"></param>
        public static void QueryLoginStatus(long account_idx, Action<eLoginResult> callback)
        {
	        string sql = "call SP_LOGIN_ACCOUNT(" + account_idx + "," + 1 + ")";
            DBManager.Instance.GetDB(eDBType.Member, 0).Query(sql, (reader) =>
                {
                    eLoginResult login_ret = eLoginResult.E_FAILED_UNKNOWNERROR;
                    if (reader.HasRows && reader.Read())
                    {
                        uint login_status = reader.GetUInt32(0);
                        uint error = reader.GetUInt32(1);
	                    if (error == 0)
	                    {
		                    if (login_status == 0)
			                    login_ret = eLoginResult.E_SUCCESS;
		                    else
			                    login_ret = eLoginResult.E_FAILED_ALREADYLOGIN;
	                    }
	                    else
	                    {
		                    login_ret = eLoginResult.E_FAILED_SERVERINTERNALERROR;
	                    }
                    }
                    callback(login_ret);
                }
            );
        }
        /// <summary>
        /// 登出
        /// </summary>
        public static void LoginOut(long account_idx)
        {
            string sql = "call SP_LOGOUT_ACCOUNT(" + account_idx + ")";
            DBManager.Instance.GetDB(eDBType.Member, 0).Execute(sql);
        }
    }
}
