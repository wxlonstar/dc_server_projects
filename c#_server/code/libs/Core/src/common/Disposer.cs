using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 非托管对象释放
    /// @author hannibal
    /// @time 2018-1-2
    /// </summary>
    public abstract class Disposer : IDisposable
    {
        public virtual void Dispose()
        {

        }
    }
}