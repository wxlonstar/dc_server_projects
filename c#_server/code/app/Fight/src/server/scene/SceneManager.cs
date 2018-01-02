using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 场景管理器
    /// @author hannibal
    /// @time 2017-8-14
    /// </summary>
    public class SceneManager : Singleton<SceneManager>
    {
        private uint m_ShareGUID = 0;
        private Dictionary<uint, BaseScene> m_DicScenes;

        public SceneManager()
        {
            m_DicScenes = new Dictionary<uint, BaseScene>();
        }

        public void Setup()
        {

        }
        public void Destroy()
        {
            foreach(var obj in m_DicScenes)
            {
                obj.Value.Destroy();
            }
            m_DicScenes.Clear();
        }
        public void Tick()
        {

        }

        public BaseScene CreateScene(ushort scene_type)
        {
            if (m_DicScenes.ContainsKey(scene_type))
                return null;

            BaseScene scene = new BaseScene();
            scene.SceneGUID = ++m_ShareGUID;
            scene.Setup(scene_type);
            m_DicScenes.Add(scene.SceneGUID, scene);

            return scene;
        }

        public void DestroyScene(uint scene_id)
        {
            BaseScene scene = null;
            if (m_DicScenes.TryGetValue(scene_id, out scene))
            {
                scene.Destroy();
            }
            m_DicScenes.Remove(scene_id);
        }
    }
}
