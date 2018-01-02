using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class aoi
    {
        public const int REGION_ROWS = 10;  //aoi区域大小
        public const int REGION_COLS = 10;

    }
    /// <summary>
    /// aoi对象信息
    /// </summary>
    public class AOIUnitInfo : IPoolsObject
    {
        public long char_idx;
        public long scene_idx;
        public int row;
        public int col;
        public List<long> observer_units = new List<long>();
        public void Init()
        {
            observer_units.Clear();
        }
    }
}
