using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 发给客户端的事件，只能针对在线玩家发送；需要离线发送的，请走邮件流程
    /// </summary>
    public enum eClientEvent
    {
        
    }
    /// <summary>
    /// 事件产生的操作行为
    /// </summary>
    public enum eClientEventAction
    {
        Unknow,
    }
}
