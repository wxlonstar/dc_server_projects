using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class login
    {
    }

    /// <summary>
    /// 登录结果
    /// </summary>
    public enum eLoginResult
    {
        E_SUCCESS = 0,	// success
        E_SUCCESS_QUEUEDLOGIN,					// 登录成功,进入队列.
        E_SUCCESS_QUEUECOMPLETE,				// 队列完成,进入游戏.
        E_FAILED_ALREADYLOGIN,					// 同帐号已经登录
        E_FAILED_SERVERINTERNALERROR,			//	e.g. no available ls/dp server
        E_FAILED_INVALIDACCOUNTORPASSWORD,
        E_FAILED_INVALIDACCOUNT,
        E_FAILED_INVALIDPASSWORD,
        E_FAILED_ACCOUNTDISABLED,				// 帐号已经封停
        E_FAILED_INVALIDVERSION,				// 错误的版本号
        E_FAILED_ONLINELIMIT,					// 服务器人数已满
        E_FAILED_LOGINQUEUE_IS_FULL,			// 连接已满
        E_FAILED_LOGIN_TIMEOUT,					// 登录超时
        E_FAILED_NOT_ALLOWED,					// 不允许操作
        E_FAILED_LIMIT_INVALID_PASSWORD,		// 密码错误次数太多
        E_FAILED_NOT_INPUT_ACCOUNT_NAME,		// 未输入账号名
        E_FAILED_IP_BLOCK,						// IP 被封
        E_FAILED_UNKNOWNERROR,                  // 未知错误
    }

    /// <summary>
    /// 登录类型
    /// </summary>
    public enum eLoginType
    {
        LT_DEFAULT,				// 默认类型,由GS根据配置决定
        LT_NAIVE,				// 原始明文密码
        LT_PASSWORD_MD5,		// md5加密密码
        LT_AUTHENTICATION,      // 后台验证
    }

    /// <summary>
    /// 帐号数据
    /// </summary>
	public struct AccountData
	{
		public long account_idx;
		public string password_md5;
		public byte account_type;
        public ushort spid;
	};
}
