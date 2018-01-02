using System;
using System.Collections.Generic;

namespace dc
{
    public class ServerMsgSend
    {
        private static bool CheckLogin()
        {
            return true;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name">账号</param>
        /// <param name="psw">密码</param>
        public static void SendLogin(long conn_idx, string name, string psw)
        {
            c2gs.ClientLogin msg = PacketPools.Get(c2gs.msg.CLIENT_LOGIN) as c2gs.ClientLogin;
            msg.name = name;
            msg.psw = psw;
            ClientNetManager.Instance.Send(conn_idx, msg);
        }
        /// <summary>
        /// 请求角色列表
        /// </summary>
        public static void SendCharacterList(long conn_idx)
        {
            if (!CheckLogin()) return;

            c2gs.EnumCharacter msg = PacketPools.Get(c2gs.msg.ENUM_CHAR) as c2gs.EnumCharacter;
            ClientNetManager.Instance.Send(conn_idx, msg);
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        public static void SendCreateCharacter(long conn_idx, string name, uint flag)
        {
            if (!CheckLogin()) return;

            c2gs.CreateCharacter msg = PacketPools.Get(c2gs.msg.CREATE_CHARACTER) as c2gs.CreateCharacter;
            msg.name = name;
            msg.flags = flag;
            ClientNetManager.Instance.Send(conn_idx, msg);
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        public static void SendEnterGame(long conn_idx, long char_idx)
        {
            if (!CheckLogin()) return;

            c2gs.EnterGame msg = PacketPools.Get(c2gs.msg.ENTER_GAME) as c2gs.EnterGame;
            msg.char_idx = char_idx;
            ClientNetManager.Instance.Send(conn_idx, msg);
        }
        /// <summary>
        /// 场景切换
        /// </summary>
        public static void SendEnterScene(long conn_idx, uint scene_type)
        {
            if (!CheckLogin()) return;

            c2ss.EnterScene msg = PacketPools.Get(c2ss.msg.ENTER_SCENE) as c2ss.EnterScene;
            msg.scene_type = scene_type;
            ClientNetManager.Instance.Send(conn_idx, msg);
        }
        /// <summary>
        /// 移动
        /// </summary>
        public static void SendUnitMove(long conn_idx, int x, int y)
        {
            if (!CheckLogin()) return;

            c2ss.UnitMove msg = PacketPools.Get(c2ss.msg.UNIT_MOVE) as c2ss.UnitMove;
            msg.pos.Set(x, y);
            ClientNetManager.Instance.Send(conn_idx, msg);
        }
    }
}
