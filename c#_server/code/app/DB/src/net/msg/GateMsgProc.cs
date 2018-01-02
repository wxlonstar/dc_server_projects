using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 消息处理
    /// @author hannibal
    /// @time 2017-8-20
    /// </summary>
    public class GateMsgProc : ConnAppProc
    {
        public GateMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_GS2DB;
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
        }
        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public override int Send(PacketBase packet)
        {
            return ForServerNetManager.Instance.Send(m_conn_idx, packet);
        }
        protected override void RegisterHandle()
        {
            RegisterMsgProc(gs2db.msg.CLIENT_LOGIN, OnClientLogin);
            RegisterMsgProc(gs2db.msg.ROBOT_TEST, OnRobotTest);
        }

        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }

        private void OnClientLogin(PacketBase packet)
        {
            gs2db.ClientLogin msg = packet as gs2db.ClientLogin;

            ClientUID client_uid = msg.client_uid;
            string account_name = msg.name;
            string account_psw = msg.psw;
            SQLLoginHandle.QueryAccountData(account_name, (data) =>
            {
                ///1.验证账号密码
                eLoginResult result = eLoginResult.E_FAILED_UNKNOWNERROR;
                if(data.account_idx > 0)
                {
                    string md5_msg = StringUtils.GetMD5(account_psw);
                    if (data.password_md5 == md5_msg)
                        result = eLoginResult.E_SUCCESS;
                    else
                        result = eLoginResult.E_FAILED_INVALIDPASSWORD;
                }
                else
                    result = eLoginResult.E_FAILED_INVALIDACCOUNT;

                //if(result != eLoginResult.E_SUCCESS)
                {
                    db2gs.ClientLogin rep_msg = PacketPools.Get(db2gs.msg.CLIENT_LOGIN) as db2gs.ClientLogin;
                    rep_msg.client_uid = client_uid;
                    rep_msg.result = result;
                    rep_msg.account_idx = data.account_idx;
                    rep_msg.spid = data.spid;
                    this.Send(rep_msg);
                    return;
                }

                /////2.验证登录状态
                //DBHandle.QueryLoginStatus(data.account_idx, (login_res) =>
                //{
                //    db2gs.ClientLogin rep_msg = PacketPools.Get(db2gs.msg.CLIENT_LOGIN) as db2gs.ClientLogin;
                //    rep_msg.client_uid = msg.client_uid;
                //    rep_msg.result = login_res;
                //    rep_msg.account_idx = data.account_idx;
                //    this.Send(rep_msg);
                //}
                //);
            }
            );
        }
        /// <summary>
        /// 压力测试
        /// </summary>
        private void OnRobotTest(PacketBase packet)
        {
            gs2db.RobotTest msg = packet as gs2db.RobotTest;

            db2gs.RobotTest rep_msg = PacketPools.Get(db2gs.msg.ROBOT_TEST) as db2gs.RobotTest;
            rep_msg.client_uid = msg.client_uid;
            rep_msg.length = msg.length;
            this.Send(rep_msg);
        }
    }
}
