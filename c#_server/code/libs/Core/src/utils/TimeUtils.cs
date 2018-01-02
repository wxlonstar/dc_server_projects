using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 时间工具类，内部使用，逻辑层统一使用Time.cs
    /// @author hannibal
    /// @time 2014-11-14
    /// </summary>
    public class TimeUtils
    {
        /// <summary>
        /// 1970到现在的毫秒数
        /// </summary>
        static public long TimeSince1970
        {
            get
            {
                System.TimeSpan duration = System.DateTime.Now - DateTime.Parse("1970-1-1");
                return (long)duration.TotalMilliseconds;
            }
        }
        /// <summary>
        /// 2009到现在的毫秒数
        /// </summary>
        static public long TimeSince2009
        {
            get
            {
                System.TimeSpan duration = System.DateTime.Now - DateTime.Parse("2009-1-1");
                return (long)duration.TotalMilliseconds;
            }
        }
        /// <summary>
        /// 1970到现在的秒数
        /// </summary>
        static public long SecondSince1970
        {
            get
            {
                System.TimeSpan duration = System.DateTime.Now - DateTime.Parse("1970-1-1");
                return (long)duration.TotalSeconds;
            }
        }
        /// <summary>
        /// 2009到现在的秒数
        /// </summary>
        static public long SecondSince2009
        {
            get
            {
                System.TimeSpan duration = System.DateTime.Now - DateTime.Parse("2009-1-1");
                return (long)duration.TotalSeconds;
            }
        }
        /**获取当前时间  “年” “月” “日” “时” “分”*/
        static public string GetNowTime(out UInt32 year,
                                        out UInt32 month,
                                        out UInt32 day,
                                        out UInt32 hour,
                                        out UInt32 min)
        {
            System.DateTime baseDate = new System.DateTime(2009, 1, 1);
            System.TimeSpan duration = System.DateTime.Now - baseDate;
            return GetTimeSince2009((UInt32)duration.TotalSeconds,
                                    out year,
                                    out month,
                                    out day,
                                    out hour,
                                    out min);
        }

        static public string GetNowTime()
        {
            System.DateTime baseDate = new System.DateTime(2009, 1, 1);
            System.TimeSpan duration = System.DateTime.Now - baseDate;
            return GetTimeSince2009((UInt32)duration.TotalSeconds);
        }

        static public string GetTimeSince2009(UInt32 second,
                                              out UInt32 year,
                                              out UInt32 month,
                                              out UInt32 day,
                                              out UInt32 hour,
                                              out UInt32 min)
        {
            System.DateTime baseDate = new System.DateTime(2009, 1, 1);
            baseDate = baseDate.AddSeconds(second);
            year = (UInt32)baseDate.Year;
            month = (UInt32)baseDate.Month;
            day = (UInt32)baseDate.Day;
            hour = (UInt32)baseDate.Hour;
            min = (UInt32)baseDate.Minute;
            return baseDate.ToString("yyyy-MM-dd-hh:mm");
        }

        static public string GetTimeSince2009(UInt32 second)
        {
            System.DateTime baseDate = new System.DateTime(2009, 1, 1);
            baseDate = baseDate.AddSeconds(second);
            // 调整为当前系统时区
            baseDate = baseDate.ToLocalTime();
            return baseDate.ToString("yyyy-MM-dd-hh:mm");
        }

        static public string GetTimeSince1970(UInt32 second,
                                              out UInt32 year,
                                              out UInt32 month,
                                              out UInt32 day,
                                              out UInt32 hour,
                                              out UInt32 min)
        {
            System.DateTime baseDate = new System.DateTime(1970, 1, 1);
            baseDate = baseDate.AddSeconds(second);
            // 调整为当前系统时区
            baseDate = baseDate.ToLocalTime();
            year = (UInt32)baseDate.Year;
            month = (UInt32)baseDate.Month;
            day = (UInt32)baseDate.Day;
            hour = (UInt32)baseDate.Hour;
            min = (UInt32)baseDate.Minute;
            return baseDate.ToString("yyyy-MM-dd-hh:mm");
        }


        static public string GetTimeSince1970(UInt32 second)
        {
            System.DateTime baseDate = new System.DateTime(1970, 1, 1);
            baseDate = baseDate.AddSeconds(second);
            // 调整为当前系统时区
            baseDate = baseDate.ToLocalTime();
            return baseDate.ToString("yyyy-MM-dd-hh:mm");
        }
    }
}
