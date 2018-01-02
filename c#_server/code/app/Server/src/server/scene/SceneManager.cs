using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 场景管理器
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public class SceneManager : Singleton<SceneManager>
    {
        private long m_scene_guid = 0;
        private Dictionary<long, BaseScene> m_scenes;
        private Dictionary<uint, BaseScene> m_city_scenes;

        public SceneManager()
        {
            m_scenes = new Dictionary<long, BaseScene>();
            m_city_scenes = new Dictionary<uint, BaseScene>();
        }

        public void Setup()
        {

        }
        public void Destroy()
        {
            foreach(var obj in m_scenes)
            {
                obj.Value.Destroy();
            }
            m_scenes.Clear();
        }
        public void Tick()
        {

        }
        /// <summary>
        /// 创建场景
        /// </summary>
        /// <param name="scene_type"></param>
        /// <returns></returns>
        public BaseScene CreateScene(uint scene_type)
        {
            BaseScene new_scene = null;
            eSceneType type = SceneID.GetSceneTypeByID(scene_type);
            if (type == eSceneType.CITY)
            {//主城场景:同类型只存在一个
                if (m_city_scenes.TryGetValue(scene_type, out new_scene))
                    return new_scene;
            }
            //创建场景
            switch(type)
            {
                case eSceneType.CITY: new_scene = new CityScene(); break;
                case eSceneType.BATTLE: new_scene = new BaseScene(); break;
                default: Log.Warning("错误的场景类型:" + scene_type); return null;
            }
            new_scene.Setup(scene_type, ++m_scene_guid);
            m_scenes.Add(new_scene.scene_obj_idx, new_scene);

            if (type == eSceneType.CITY)
            {
                m_city_scenes.Add(scene_type, new_scene);
            }
            else if(type == eSceneType.BATTLE)
            {

            }

            return new_scene;
        }

        public void DestroyScene(long scene_obj_idx)
        {
            BaseScene scene = null;
            if (m_scenes.TryGetValue(scene_obj_idx, out scene))
            {
                scene.Destroy();
            }
            m_scenes.Remove(scene_obj_idx);
        }
        public BaseScene GetScene(long scene_obj_idx)
        {
            BaseScene _scene = null;
            if (m_scenes.TryGetValue(scene_obj_idx, out _scene))
            {
                return _scene;
            }
            return null;
        }
        public BaseScene GetScene(ushort scene_type)
        {
            BaseScene _scene = null;
            eSceneType type = SceneID.GetSceneTypeByID(scene_type);
            if(type == eSceneType.CITY)
            {
                if (m_city_scenes.TryGetValue(scene_type, out _scene))
                {
                    return _scene;
                }
                return null;
            }
            else if(type == eSceneType.BATTLE)
            {
                return null;
            }
            return null;
        }
    }
}
