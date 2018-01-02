using System;
using System.Collections.Generic;

namespace dc
{
    public class cmd
    {
        public static string[] list = 
        {
            "",
            "help",
            "clr",
            "reload",
            "pools",
            "shutdown",
            "online",
            "loglv"
        };
        public static string[] tips = 
        {
            "",
            "\t\t帮助",
            "\t\t清除控制台",
            "\t重新加载配置表",
            "\t\t对象池信息",
            "\t关服(参数1:可选，关服等待时间，不指定使用默认等待时间)",
            "\t在线人数统计",
            "\t日志等级(1:DEBUG,2:INFO,3:WARNING,4:ERROR,5:EXCEPTION)"
        };
    }
    public enum eCmdType
    {
        Help = 1,
        Clear,          //清除控制台
        Reload,         //重新加载配置表
        Pools,          //对象池信息
        Shutdown,       //关服
        Online,         //在线人数统计
        LogLv,          //日志等级
        Max
    }
}
