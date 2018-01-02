using System;
using System.Collections.Generic;

namespace dc
{
    public class DBUtils
    {
        private static ByteArray tmpDBArray = new ByteArray(1024*8, 65535);
        public static ByteArray AllocDBArray()
        {
            tmpDBArray.Clear();
            return tmpDBArray;
        }
    }
}
