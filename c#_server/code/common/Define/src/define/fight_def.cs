using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 战斗
    /// @author hannibal
    /// @time 2017-10-17
    /// </summary>
    public class fight
    {
    }
    /// <summary>
    /// 战斗阶段
    /// </summary>
    public enum eFightStage
    {
        None,
        Match,          //匹配中
        WaitingFight,   //匹配成功，等待开始战斗
        Fighting,       //战斗中
        ClearBattle,    //打扫战场
        Finish,         //战场结束，切回主城
    }
    /// <summary>
    /// 战斗服连接状态
    /// </summary>
    public enum eFSConnectState
    {
        UnConnect,
        Connecting,
        Connected,
    }
}
