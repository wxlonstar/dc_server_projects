using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 战斗
    /// @author hannibal
    /// @time 2017-8-14
    /// </summary>
    public class FightManager : Singleton<FightManager>
    {
        private Dictionary<long, FightPlayer> m_fight_players = null;//所有战斗玩家集合

        public FightManager()
        {
            m_fight_players = new Dictionary<long, FightPlayer>();
        }

        public void Setup()
        {
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            foreach (var obj in m_fight_players)
            {
                obj.Value.Destroy();
                CommonObjectPools.Despawn(obj.Value);
            }
            m_fight_players.Clear();
        }
        public void Tick()
        {
            foreach (var obj in m_fight_players)
            {
                obj.Value.Update();
            }
        }

        #region 事件
        private void RegisterEvent()
        {
            EventController.AddEventListener(EventID.PLAYER_ENTER_GAME, OnGameEvent);
            EventController.AddEventListener(EventID.PLAYER_LEAVE_GAME, OnGameEvent);
            EventController.AddEventListener(EventID.NET_DISCONNECT_FIGHT_SRV, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(EventID.PLAYER_ENTER_GAME, OnGameEvent);
            EventController.RemoveEventListener(EventID.PLAYER_LEAVE_GAME, OnGameEvent);
            EventController.RemoveEventListener(EventID.NET_DISCONNECT_FIGHT_SRV, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case EventID.PLAYER_ENTER_GAME:
                    {
                        long char_idx = evt.Get<long>(0);
                        this.OnPlayerLogin(char_idx);
                    }
                    break;
                case EventID.PLAYER_LEAVE_GAME:
                    {
                        long char_idx = evt.Get<long>(0);
                        this.OnPlayerLogout(char_idx);
                    }
                    break;
                case EventID.NET_DISCONNECT_FIGHT_SRV:
                    {
                        ushort server_uid = evt.Get<ushort>(0);
                        this.OnFightServerLeave(server_uid);
                    }
                    break;
            }
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        private void OnPlayerLogin(long char_idx)
        {
            //如果是登陆进入游戏，判断是否还处于战场中；如果是，需要重连战斗
            Player player = UnitManager.Instance.GetPlayerByIdx(char_idx);
            if (player != null && player.fs_uid > 0)
            {
                this.AddPlayer(char_idx);
                FightPlayer f_player = null;
                if (m_fight_players.TryGetValue(char_idx, out f_player))
                {
                    f_player.EnterFight();
                }
            }
        }
        /// <summary>
        /// 玩家登出
        /// </summary>
        private void OnPlayerLogout(long char_idx)
        {
            this.RemovePlayer(char_idx);
        }
        /// <summary>
        /// 战斗服退出
        /// </summary>
        /// <param name="uid"></param>
        private void OnFightServerLeave(ushort uid)
        {
            FightPlayer player = null;
            foreach (var obj in m_fight_players)
            {
                player = obj.Value as FightPlayer;
                if (player != null)
                {
                    player.HandleFightServerLeave(uid);
                }
            }
        }
        #endregion

        #region 集合管理
        public bool AddPlayer(long char_idx)
        {
            if (m_fight_players.ContainsKey(char_idx))
            {
                RemovePlayer(char_idx);
            }
            FightPlayer player = CommonObjectPools.Spawn<FightPlayer>();
            player.Setup(char_idx);
            m_fight_players.Add(char_idx, player);

            return true;
        }
        public void RemovePlayer(long char_idx)
        {
            FightPlayer player = null;
            if (m_fight_players.TryGetValue(char_idx, out player))
            {
                player.Destroy();
                CommonObjectPools.Despawn(player);
            }
            m_fight_players.Remove(char_idx);
        }

        public FightPlayer GetPlayerByIdx(long char_idx)
        {
            FightPlayer player = null;
            if (m_fight_players.TryGetValue(char_idx, out player))
                return player;
            return null;
        }
        #endregion
    }
}
