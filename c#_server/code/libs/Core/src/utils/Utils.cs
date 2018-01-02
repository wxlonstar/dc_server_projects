using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace dc
{
    /// <summary>
    /// 公用方法
    /// @author hannibal
    /// @time 2014-11-14
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// 函数调用堆栈
        /// </summary>
        public static string GetStackTrace()
        {
            string stackInfo = new System.Diagnostics.StackTrace().ToString();
            return stackInfo;
        }
        /// <summary>
        /// 给一个字符串进行MD5加密
        /// </summary>
        /// <param name="strText">待加密字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(strText));
            string str16 = String.Empty;
            for (int i = 0; i < result.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                str16 = str16 + result[i].ToString("X");
            }
            return str16;
        }
        /// <summary>
        /// 对文件加密
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static string MD5ByPathName(string pathName)
        {
            try
            {
                FileStream file = new FileStream(pathName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return "";
        }

        public static bool HasFlag(uint a, uint b)
        {
            return ((a & b) == 0) ? false : true;
        }
        public static uint InsertFlag(uint a, uint b)
        {
            a |= b;
            return a;
        }
        public static uint RemoveFlag(uint a, uint b)
        {
            a ^= b;
            return a;
        }
        /// <summary>
        /// 天数转毫秒
        /// </summary>
        public static long Day2Millisecond(int day)
        {
            return day * (1000 * 60 * 60 * 24);
        }
        /// <summary>
        /// 天数转秒
        /// </summary>
        public static long Day2Second(int day)
        {
            return day * (60 * 60 * 24);
        }
        /// <summary>
        /// 小时转毫秒
        /// </summary>
        public static long Hour2Millisecond(int hour)
        {
            return hour * (1000 * 60 * 60);
        }
    }
}
