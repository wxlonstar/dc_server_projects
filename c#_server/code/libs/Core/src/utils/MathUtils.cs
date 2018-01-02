using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 数学
    /// @author hannibal
    /// @time 2014-11-1
    /// </summary>
    public class MathUtils
    {
        /**字节转换M*/
        static public float BYTE_TO_M = 1.0f / (1024 * 1024);
        /**字节转换K*/
        static public float BYTE_TO_K = 1.0f / (1024);

        public static float Min(float first, float second)
        {
            return (first < second ? first : second);
        }
        public static float Max(float first, float second)
        {
            return (first > second ? first : second);
        }
        /**
         * 获得一个数的符号(-1,0,1)
         * 结果：大于0：1，小于0：-1，等于0:0
         */
        public static int Sign(float value)
        {
            return (value < 0 ? -1 : (value > 0 ? 1 : 0));
        }
        static private Random ro = new Random();
        /// <summary>
        /// 产生随机数
        /// </summary>
        /// <returns>结果：x>=0 && x<1</returns>
        public static float Range()
        {
            return ((float)ro.Next(0, int.MaxValue)) / ((float)int.MaxValue);
        }
        /// <summary>
        /// 产生随机数
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns>x>=param1 && x<param2</returns>
        public static int RandRange(int param1, int param2)
        {
            return ro.Next(param1, param2);
        }
        /**
         * 从数组中产生随机数[-1,1,2]
         * 结果：-1/1/2中的一个
         */
        public static T RandRange_Array<T>(T[] arr)
        {
            T loc = arr[RandRange(0, arr.Length)];
            return loc;
        }
        public static T RandRange_List<T>(List<T> arr)
        {
            T loc = arr[RandRange(0, arr.Count)];
            return loc;
        }
        /**
         * 随机1/-1
         * 结果：1/-1
         */
        public static int Rand_Sign()
        {
            int[] arr = new int[2] { -1, 1 };
            int loc = RandRange_Array<int>(arr);
            return loc;
        }
        /// <summary>
        /// 限制范围
        /// </summary>
        public static float Clamp(float n, float min, float max)
        {
            if (min > max)
            {
                var i = min;
                min = max;
                max = i;
            }

            return (n < min ? min : (n > max ? max : n));
        }
        /**
         * 把一个数转换到0-360之间
         * @param num 需要转换的数
         * @return 转换后的数
         */
        public static float Cleap0_360(float num)
        {
            num = num % 360;
            num = num < 0 ? num + 360 : num;

            return num;
        }
        /**
         * 弧度转化为度 
         */
        public static float ToDegree(float radian)
        {
            return radian * (180.0f / 3.1415926f);
        }
        /// <summary>
        /// 度转化为弧度 
        /// </summary>
        public static float ToRadian(float degree)
        {
            return degree * (3.1415926f / 180.0f);
        }
    }
}
