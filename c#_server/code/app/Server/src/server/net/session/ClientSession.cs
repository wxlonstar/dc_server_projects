using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 客户端会话
    /// @author hannibal
    /// @time 2017-5-25
    /// </summary>
    public class ClientSession
    {
        private uint m_ConnID = 0;
        private eSessionStatus m_SessionStatus = eSessionStatus.INVALID;

        public void Setup(uint conn_id)
        {
            m_ConnID = conn_id;
        }
        public void Destroy()
        {

        }

        public void Update()
        {

        }

        public int Send(ByteArray by)
        {
            return ServerNetManager.Instance.Send(m_ConnID, by);
        }

        public uint ConnID
        {
            get { return m_ConnID; }
        }

        public eSessionStatus SessionStatus
        {
            get { return m_SessionStatus; }
            set { m_SessionStatus = value; }
        }
    }
    /// <summary>
    /// sessioin状态
    /// </summary>
    public enum eSessionStatus
    {
        INVALID = 0,			// 非法值
        CREATED,	            // 已创建
        LOGIN_DOING,	        // 登录中
        ALREADY_LOGIN,	        // 登录成功
        LOGIN_FAILED,	        // 登录失败
        IN_GAMING,	            // 游戏中
        DELAY_DISCONNECT,	    // 延迟断线
        LOGOUTING,				// 登出中
    }
}
