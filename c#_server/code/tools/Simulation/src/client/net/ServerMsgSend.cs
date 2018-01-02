using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class ServerMsgSend
    {
        private static bool CheckLogin()
        {
            if (!ClientNetManager.Instance.is_connected)
            {
                EventController.TriggerEvent(ClientEventID.SHOW_STATUS, eFormStatusType.Log, "请先登录");
                return false;
            }
            return true;
        }
        #region ping
        /// <summary>
        /// ping gate
        /// </summary>
        public static void SendPingGS(uint packet_idx)
        {
            c2gs.PingNet msg = PacketPools.Get(c2gs.msg.PING_NET) as c2gs.PingNet;
            msg.packet_id = packet_idx;
            msg.tick = Time.time;
            ClientNetManager.Instance.Send(msg);
        }
        public static void SendPingSS(uint packet_idx)
        {
            c2ss.PingNet msg = PacketPools.Get(c2ss.msg.PING_NET) as c2ss.PingNet;
            msg.packet_id = packet_idx;
            msg.tick = Time.time;
            msg.flags = 0;
            ClientNetManager.Instance.Send(msg);
        }
        public static void SendPingFS(uint packet_idx)
        {
            c2fs.PingNet msg = PacketPools.Get(c2fs.msg.PING_NET) as c2fs.PingNet;
            msg.packet_id = packet_idx;
            msg.tick = Time.time;
            msg.flags = 0;
            ClientNetManager.Instance.Send(msg);
        }
        public static void SendPingWS(uint packet_idx)
        {
            c2ws.PingNet msg = PacketPools.Get(c2ws.msg.PING_NET) as c2ws.PingNet;
            msg.packet_id = packet_idx;
            msg.tick = Time.time;
            msg.flags = 0;
            ClientNetManager.Instance.Send(msg);
        }
        public static void SendPingGL(uint packet_idx)
        {
            c2ss.PingNet msg = PacketPools.Get(c2ss.msg.PING_NET) as c2ss.PingNet;
            msg.packet_id = packet_idx;
            msg.tick = Time.time;
            msg.flags |= (uint)eServerType.GLOBAL;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 加速检测
        /// </summary>
        public static void SendSpeedCheck(int check_sn)
        {
            c2gs.SpeedCheck msg = PacketPools.Get(c2gs.msg.SPEED_CHECK) as c2gs.SpeedCheck;
            msg.check_sn = check_sn;
            ClientNetManager.Instance.Send(msg);
        }
        #endregion
        #region 登陆
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="name">账号</param>
        /// <param name="psw">密码</param>
        public static void SendLogin(string name, string psw)
        {
            c2gs.ClientLogin msg = PacketPools.Get(c2gs.msg.CLIENT_LOGIN) as c2gs.ClientLogin;
            msg.name = name;
            msg.psw = psw;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 请求角色列表
        /// </summary>
        public static void SendCharacterList()
        {
            if (!CheckLogin()) return;

            c2gs.EnumCharacter msg = PacketPools.Get(c2gs.msg.ENUM_CHAR) as c2gs.EnumCharacter;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        public static void SendCreateCharacter(string name, uint flag)
        {
            if (!CheckLogin()) return;

            c2gs.CreateCharacter msg = PacketPools.Get(c2gs.msg.CREATE_CHARACTER) as c2gs.CreateCharacter;
            msg.name = name;
            msg.flags = flag;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        public static void SendEnterGame(long char_idx)
        {
            if (!CheckLogin()) return;

            c2gs.EnterGame msg = PacketPools.Get(c2gs.msg.ENTER_GAME) as c2gs.EnterGame;
            msg.char_idx = char_idx;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 场景切换
        /// </summary>
        public static void SendEnterScene(uint scene_type)
        {
            if (!CheckLogin()) return;

            c2ss.EnterScene msg = PacketPools.Get(c2ss.msg.ENTER_SCENE) as c2ss.EnterScene;
            msg.scene_type = scene_type;
            ClientNetManager.Instance.Send(msg);
        }
        #endregion
        #region 角色
        /// <summary>
        /// 移动
        /// </summary>
        public static void SendUnitMove(int x, int y)
        {
            if (!CheckLogin()) return;

            c2ss.UnitMove msg = PacketPools.Get(c2ss.msg.UNIT_MOVE) as c2ss.UnitMove;
            msg.pos.Set(x, y);
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        public static void SendUnitModifyInt(eUnitModType type, long value)
        {
            if (!CheckLogin()) return;

            c2ss.NotifyUpdatePlayerAttribInteger msg = PacketPools.Get(c2ss.msg.UNIT_MODIFY_INT) as c2ss.NotifyUpdatePlayerAttribInteger;
            msg.type = type;
            msg.value = value;
            ClientNetManager.Instance.Send(msg);
        }
        public static void SendUnitModifyString(eUnitModType type, string value)
        {
            if (!CheckLogin()) return;

            c2ss.NotifyUpdatePlayerAttribString msg = PacketPools.Get(c2ss.msg.UNIT_MODIFY_STRING) as c2ss.NotifyUpdatePlayerAttribString;
            msg.type = type;
            msg.value = value;
            ClientNetManager.Instance.Send(msg);
        }
        #endregion
        #region 邮件
        /// <summary>
        /// 请求邮件列表
        /// </summary>
        public static void SendMailList()
        {
            c2ss.MailList msg = PacketPools.Get(c2ss.msg.MAIL_LIST) as c2ss.MailList;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 读邮件
        /// </summary>
        public static void SendReadMail(long mail_idx)
        {
            c2ss.MailRead msg = PacketPools.Get(c2ss.msg.MAIL_READ) as c2ss.MailRead;
            msg.mail_idx = mail_idx;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 收取邮件附件
        /// </summary>
        public static void SendTakeMail(long mail_idx)
        {
            c2ss.MailTake msg = PacketPools.Get(c2ss.msg.MAIL_TAKE) as c2ss.MailTake;
            msg.mail_idx = mail_idx;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 删除邮件
        /// </summary>
        public static void SendDeleteMail(long mail_idx)
        {
            c2ss.MailDelete msg = PacketPools.Get(c2ss.msg.MAIL_DELETE) as c2ss.MailDelete;
            msg.mail_list.Add(mail_idx);
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 写邮件
        /// </summary>
        public static void SendWriteMail(MailWriteInfo info)
        {
            c2ss.MailWrite msg = PacketPools.Get(c2ss.msg.MAIL_WRITE) as c2ss.MailWrite;
            msg.info = info;
            ClientNetManager.Instance.Send(msg);
        }
        #endregion
        #region 关系
        /// <summary>
        /// 请求添加好友
        /// </summary>
        public static void SendRelationAdd(RelationAddTarget target, eRelationFlag flag, string message)
        {
            c2ss.RelationAdd msg = PacketPools.Get(c2ss.msg.RELATION_ADD) as c2ss.RelationAdd;
            msg.target_id = target;
            msg.message = message;
            msg.flag = flag;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 移除关系
        /// </summary>
        public static void SendRelationRemove(long char_idx)
        {
            c2ss.RelationRemove msg = PacketPools.Get(c2ss.msg.RELATION_REMOVE) as c2ss.RelationRemove;
            msg.target_char_idx = char_idx;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 请求好友操作
        /// </summary>
        public static void SendRelationApplyCommand(long event_idx, long char_idx, eRelationApplyCmd cmd)
        {
            c2ss.RelationApplyCmd msg = PacketPools.Get(c2ss.msg.RELATION_APPLY_CMD) as c2ss.RelationApplyCmd;
            msg.event_idx = event_idx;
            msg.target_char_idx = char_idx;
            msg.cmd = cmd;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 请求列表
        /// </summary>
        public static void SendRelationList()
        {
            c2ss.RelationList msg = PacketPools.Get(c2ss.msg.RELATION_LIST) as c2ss.RelationList;
            ClientNetManager.Instance.Send(msg);
        }
        /// <summary>
        /// 赠送
        /// </summary>
        public static void SendRelationGive(long char_idx, ItemID item_id)
        {
            c2ss.RelationGive msg = PacketPools.Get(c2ss.msg.RELATION_GIVE) as c2ss.RelationGive;
            msg.target_char_idx = char_idx;
            msg.item_id = item_id;
            ClientNetManager.Instance.Send(msg);
        }
        #endregion
        #region 聊天
        /// <summary>
        /// 聊天
        /// </summary>
        public static void SendChat(eChatType type, long receiver, string content)
        {
            c2ss.ChatSend msg = PacketPools.Get(c2ss.msg.CHAT_SEND) as c2ss.ChatSend;
            msg.type = type;
            msg.chat_content = content;
            if (type == eChatType.PRIVATE)
                msg.receiver.SetIdx(receiver);
            ClientNetManager.Instance.Send(msg);
        }
        #endregion
        #region ws
        /// <summary>
        /// 请求服务器时间
        /// </summary>
        public static void SendServerTime()
        {
            c2ws.ServerTime msg = PacketPools.Get(c2ws.msg.SERVER_TIME) as c2ws.ServerTime;
            ClientNetManager.Instance.Send(msg);
        }
        #endregion
    }
}
