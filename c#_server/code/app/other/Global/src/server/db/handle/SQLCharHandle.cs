using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace dc
{
    /// <summary>
    /// 角色操作
    /// @author hannibal
    /// @time 2016-9-1
    /// </summary>
    public class SQLCharHandle
    {
        /// <summary>
        /// 查询需要预加载的列表
        /// </summary>
        public static void QueryCharForPreload(Action<List<long>> callback)
        {
            List<long> list = new List<long>();
            string sql = "call SP_CHARACTER_FOR_PRELOAD";
            DBManager.Instance.GetDB(eDBType.Center, 0).Query(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        long char_idx = reader.GetInt64(0);
                        list.Add(char_idx);
                    }
                }
                callback(list);
            });
        }
        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public static void QueryCharacterInfo(long char_idx, PlayerInfoForGL data, Action<bool> callback)
        {
            bool ret = false;
            string sql = "call SP_CHARACTER_BASE(" + char_idx + ")";
            DBManager.Instance.GetDB(eDBType.Center, 0).Query(sql, (reader) =>
            {
                if (reader.HasRows && reader.Read())
                {
                    int idx = 0;
                    data.char_idx = char_idx;
                    data.char_name = reader.GetString(idx++);
                    data.char_type = reader.GetByte(idx++);
                    data.spid = reader.GetUInt16(idx++);
                    data.ws_id = reader.GetUInt16(idx++);
                    data.scene_type_idx = reader.GetUInt32(idx++);
                    data.flags = reader.GetUInt32(idx++);
                    data.model_idx = reader.GetUInt32(idx++);
                    data.job = reader.GetByte(idx++);
                    data.level = reader.GetUInt16(idx++);
                    data.exp = reader.GetUInt32(idx++);
                    data.gold = reader.GetUInt32(idx++);
                    data.coin = reader.GetUInt32(idx++);
                    data.vip_grade = reader.GetUInt32(idx++);
                    ret = true;
                }
                else
                {
                    Log.Warning("查询角色失败:" + char_idx);
                }
                callback(ret);
            });
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="info"></param>
        public static void SaveCharacterInfo(PlayerInfoForGL info)
        {
            string sql = ("replace into `character` (`char_index`,`char_name`,`char_type`,`spid`,`ws_id`,`scene_type_idx`,`flags`,`model_idx`,`job`,`level`,`exp`,`gold`,`coin`,`vip_grade`,`time_last_logout`) values(" +
                info.char_idx + ",'" + 
                info.char_name + "'," +             // 名字
                info.char_type + "," +              // 角色类型(男 or 女 )
                info.spid + "," +                   // 
                info.ws_id + "," +                  // 
                info.scene_type_idx + "," +
                info.flags + "," +                  // 特殊标记
                info.model_idx + "," +              // 模型ID
                info.job + "," +                    // 职业
                info.level + "," +                  // 角色等级
                info.exp + "," +                    // 当前经验
                info.gold + "," +                   // 金币（点卷）
                info.coin + "," +                   // 游戏币(铜币)
                info.vip_grade + "," +
                "now());"
                );
            DBManager.Instance.GetDB(eDBType.Center, 0).Execute(sql);
        }
    }
}
