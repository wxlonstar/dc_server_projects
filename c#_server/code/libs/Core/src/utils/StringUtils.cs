using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace dc
{
    /// <summary>
    /// 字符串
    /// @author hannibal
    /// @time 2014-11-14
    /// </summary>
    public class StringUtils
    {
        //～～～～～～～～～～～～～～～～～～～～～～～基础方法~～～～～～～～～～～～～～～～～～～～～～～～//
        /// <summary>
        /// 是否为整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInteger(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            long value;
            return long.TryParse(str, out value);
        }
        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetMD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));
            }
            return strbul.ToString();
        }
        //～～～～～～～～～～～～～～～～～～～～～～～字符串处理~～～～～～～～～～～～～～～～～～～～～～～～//
        /**获取两个字符串中间的字符串*/
        static public string Search_string(string s, string s1, string s2)
        {
            int n1, n2;
            n1 = s.IndexOf(s1, 0);  //开始位置 
            if (n1 < 0) return "";
            n1 += s1.Length;
            n2 = s.IndexOf(s2, n1);             //结束位置  
            if (n2 < 0) return "";
            return s.Substring(n1, n2 - n1);   	//取搜索的条数，用结束的位置-开始的位置,并返回  
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="split">分割符</param>
        public static List<T> Split<T>(string str, char split)
        {
            List<T> tmpList = new List<T>();
            if (string.IsNullOrEmpty(str)) return tmpList;

            string[] strArr = str.Split(split);
            if (typeof(T) == typeof(int))
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    tmpList.Add((T)(object)int.Parse(strArr[i]));
                }
            }
            else if (typeof(T) == typeof(uint))
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    tmpList.Add((T)(object)uint.Parse(strArr[i]));
                }
            }
            else if (typeof(T) == typeof(float))
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    tmpList.Add((T)(object)float.Parse(strArr[i]));
                }
            }
            else if (typeof(T) == typeof(string))
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    tmpList.Add((T)(object)strArr[i]);
                }
            }
            else if (typeof(T) == typeof(byte))
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    tmpList.Add((T)(object)byte.Parse(strArr[i]));
                }
            }
            else if (typeof(T) == typeof(short))
            {
                for (int i = 0; i < strArr.Length; i++)
                {
                    tmpList.Add((T)(object)short.Parse(strArr[i]));
                }
            }
            else
            {
                Log.Error("Split : type error");
            }

            return tmpList;
        }
        //～～～～～～～～～～～～～～～～～～～～～～～time~～～～～～～～～～～～～～～～～～～～～～～～//
        /**
         * 分钟与秒格式(如-> 40:15)
         * @param seconds
         * @return 
         */
        public static string MinuteFormat(uint seconds)
        {
            uint min = seconds / 60;
            uint sec = seconds % 60;

            string min_str = min < 10 ? ("0" + min.ToString()) : (min.ToString());
            string sec_str = sec < 10 ? ("0" + sec.ToString()) : (sec.ToString());

            return min_str + ":" + sec_str;
        }
        /**
         * 时分秒格式(如-> 05:32:20)
         * @param seconds(秒)
         * @return 
         */
        public static string HourFormat(uint seconds)
        {
            uint hour = seconds / 3600;

            string hour_str = hour < 10 ? ("0" + hour.ToString()) : (hour.ToString());

            return hour_str + ":" + MinuteFormat(seconds % 3600);
        }

    }
}
