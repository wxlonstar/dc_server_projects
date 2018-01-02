using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class SceneID
    {
        //场景id范围
        public const uint CITY_START = 1000;
        public const uint CITY_END = 9999;
        public const uint BATTLE_START = 10000;
        public const uint BATTLE_END = 99999;

        /// <summary>
        /// 根据场景id，获取场景类型
        /// </summary>
        public static eSceneType GetSceneTypeByID(uint scene_id)
        {
            if (scene_id >= CITY_START && scene_id <= CITY_END)
                return eSceneType.CITY;
            else if (scene_id >= BATTLE_START && scene_id <= BATTLE_END)
                return eSceneType.BATTLE;
            return eSceneType.NONE;
        }
        /// <summary>
        /// 是否有效场景
        /// </summary>
        public static bool IsValidScene(uint scene_id)
        {
            if ((scene_id >= SceneID.CITY_START && scene_id < SceneID.CITY_END)
                || (scene_id >= SceneID.BATTLE_START && scene_id < SceneID.BATTLE_END))
                return true;
            return false;
        }
    }

    /// <summary>
    /// 场景类型
    /// </summary>
    public enum eSceneType
    {
        NONE = 0,
        CITY,
        BATTLE,
        MAX,
    }
}
