using System;
using System.Collections.Generic;

namespace dc
{    
    /// <summary>
    /// 对象池类接口
    /// 1.需要重写Init
    /// 2.使用对象池的类，集合成员变量，需要在Init里面清空
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public interface IPoolsObject
    {
        void Init();
    }
}
