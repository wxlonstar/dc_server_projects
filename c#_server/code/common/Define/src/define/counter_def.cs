using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 次数
    /// @author hannibal
    /// @time 2016-10-11
    /// </summary>
    public class counter
    {
        /// <summary>
        /// 读取db频繁，单位秒
        /// </summary>
        public const int DB_UPDATE_TIME_OFFSET = 60 * 5;
    }
    /// <summary>
    /// 次数信息
    /// </summary>
    public struct CounterInfo
    {
        public eCounterType type;
        public ushort count;
        public long cd_time;//相对2009开始倒计时间

        public void Read(ByteArray by)
        {
            type = (eCounterType)by.ReadUShort();
            count = by.ReadUShort();
            cd_time = by.ReadLong();
        }
        public void Write(ByteArray by)
        {
            by.WriteUShort((ushort)type);
            by.WriteUShort(count);
            by.WriteLong(cd_time);
        }
    }
    /// <summary>
    /// 次数类型
    /// </summary>
    public enum eCounterType
    {
        Buy_Power = 1,      //购买体力
        Build_Camp,         //建设兵营
        Take_Coin,          //定时获取银币
    }
    /// <summary>
    /// 倒计时cd类型
    /// </summary>
    public enum eCounterCDType
    {
        Without = 1,        //不需要cd，只有次数限制
        Fixed,              //大于0数字，固定cd，等级不同，cd间隔不变，单位秒
        Increase,           //用|和,分割梯度cd，跟随等级改变而变，单位秒
        Config,             //根据配置表决定，不同等级，对应配置表不同cd，单位秒
    }
}
