using Framework.Core.Serializer;
using Framework.Core.task;
using Game.Core.EntityMgr;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System.Collections.Generic;

namespace Game.LogicServer
{

    public enum RoomState
    {
        Invalid = -1,
        Waiting = 0,
        Ready = 1,
        Started = 2,
        CheckOut = 3,
        GameOver = 4,
    }

    public enum PlayerInRoom
    {
        Invalid = -1,
        OnLook = 0,
        Sit = 1,
        Ready = 2,
        Started = 3,
        CheckOut = 4,
        GameOver = 5,
    }

    public abstract class RoomMgr
    {

        protected TimerMgr timerMgr = null;
        protected int logicServerTid = -1;
        protected int MAX_PLAYER_NUM = 4;//最大玩家数
        protected int ON_LOOK_NUM = 8;
        protected int MIN_PLAYER_READY_NUM = 2;
        protected float READY_TIME = 3.0f;
        protected int GAME_DURATION = 60 * 2;
        protected float CHECKOUT_TIME = 5.0f;
        protected bool isCheckCaculate = false;
        protected bool hasReConnectPlayer = false;
        protected float curTime = 0.0f;
        protected bool isCheckGameStarted = false;
        protected long[] playerSeats = null;
        protected long[] onLook = null;

        public static int autoId = 1;
        public int zoneId;
        public int roomId;
        public int roomState = (int)RoomState.Invalid;


        public virtual void Init(int logicServerTid, int zoneId, BaseLogicServer logicServer = null)
        {
            this.timerMgr = logicServer.TimerMgr;
            this.logicServerTid = logicServerTid;
            this.zoneId = zoneId;
            this.roomId = RoomMgr.autoId++;
            this.roomState = (int)RoomState.Waiting;
            isCheckGameStarted = false;
            isCheckCaculate = false;
            hasReConnectPlayer = false;
            curTime = 0.0f;
            playerSeats = new long[MAX_PLAYER_NUM];
            onLook = new long[ON_LOOK_NUM];
            for (int i = 0; i < ON_LOOK_NUM; i++)
            {
                if (i < MAX_PLAYER_NUM)
                {
                    playerSeats[i] = -1;
                }
                onLook[i] = -1;
            }
        }

        /// <summary>
        /// 房间逻辑的迭代
        /// </summary>
        /// <param name="dt"></param>
        public void OnUpdate(float dt)
        {
            if (hasReConnectPlayer)
            {
                hasReConnectPlayer = false;
                SendReconnectDataToPlayers();
            }

            //根据不同的房间状态执行不同的逻辑
            if (this.roomState == (int)RoomState.Waiting)
            {
                CheckStartGameCondition();
            }
            else if (this.roomState == (int)RoomState.Ready)
            {
                CountDownStartTime(dt);
            }
            else if (this.roomState == (int)RoomState.Started)
            {
                StartGameUpdate(dt);
            }
            else if (this.roomState == (int)RoomState.CheckOut)
            {
                CountDownCheckOutGame(dt);
            }
        }



        #region 游戏进程的逻辑代码

        //检查游戏是否具备开启条件
        void CheckStartGameCondition()
        {
            if (isCheckGameStarted)
            {
                isCheckGameStarted = false;
                bool ret = CheckGameStartCondition();
                if (ret)
                {
                    ReadyRoundGame();
                }
            }
        }
        //检查游戏是否具备开启条件的人数
        private bool CheckGameStartCondition()
        {
            if (roomState != (int)RoomState.Waiting)
            {
                return false;
            }
            int readyNum = 0;
            for (int i = 0; i < playerSeats.Length; i++)
            {
                if (playerSeats[i] == -1)
                {
                    continue;
                }
                GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerSeats[i]);
                if (playerEntity == null)
                {
                    continue;
                }
                if (playerEntity.uGameRoom.playerInRoomState == (int)PlayerInRoom.Ready)
                {
                    readyNum++;
                }
            }
            if (readyNum >= MIN_PLAYER_READY_NUM)
            {
                return true;
            }
            return false;
        }

        //游戏房间进入准备阶段并广播消息给房间内的玩家
        private void ReadyRoundGame()
        {
            this.roomState = (int)RoomState.Ready;
            for (int i = 0; i < onLook.Length; i++)
            {
                if (onLook[i] == -1)
                {
                    continue;
                }
                GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(onLook[i]);
                if (playerEntity != null)
                {
                    ResReadyGame res = new ResReadyGame();
                    if (playerEntity.uGameRoom.playerInRoomState == (int)PlayerInRoom.Ready)
                    {
                        playerEntity.uGameRoom.playerInRoomState = (int)PlayerInRoom.Started;
                        res.countDown = 3;
                        res.isReadyPlayer = 1;
                    }
                    else
                    {
                        res.countDown = 0;
                        res.isReadyPlayer = 0;
                    }
                    BroardcastOnLook(res);
                }
            }
        }

        //开始游戏倒计时，倒计时结束进入游戏并广播消息给房间内的玩家
        void CountDownStartTime(float dt)
        {
            curTime += dt;
            if (curTime >= READY_TIME)
            {
                curTime = 0.0f;
                this.roomState = (int)RoomState.Started;
                ResStartGame res = new ResStartGame();
                res.gameDuration = 120;
                BroardcastOnLook(res);
            }
        }

        //开始游戏
        protected abstract void StartGameUpdate(float dt);

        //结算阶段
        protected void GameEnterCheckOutStage()
        {
            curTime = 0.0f;
            this.roomState = (int)RoomState.CheckOut;
            for (int i = 0; i < playerSeats.Length; i++)
            {
                if (playerSeats[i] == -1)
                {
                    continue;
                }
                GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerSeats[i]);
                if (playerEntity != null)
                {
                    playerEntity.uGameRoom.playerInRoomState = (int)PlayerInRoom.CheckOut;
                }
            }
            isCheckCaculate = true;
        }
        //游戏结算阶段
        protected abstract void GameCheckOutStage();


        //结算阶段后游戏进程结束
        void CountDownCheckOutGame(float dt)
        {
            if (isCheckCaculate)
            {
                GameCheckOutStage();
                isCheckCaculate = false;
            }

            curTime += dt;
            if (curTime >= CHECKOUT_TIME)
            {
                curTime = 0.0f;
                this.roomState = (int)RoomState.Invalid;
                RestRoomData();
                ResGameOver res = new ResGameOver();
                res.reserve = 0;
                BroardcastOnLook(res);
            }
        }


        protected abstract int OnPlayerStartBuffInRoom(GM_PlayerEntity e, ReqStartBuff req);

        public int DoPlayerStartBuff(GM_PlayerEntity e, long playerId, ReqStartBuff req)
        {
            int status = this.OnPlayerStartBuffInRoom(e, req);
            if (status != (int)Respones.OK)
            {
                return status;
            }


            // 广播给房间里面所有的人，它释放了一个技能;
            ResStartBuff res = new ResStartBuff();
            res.buffId = req.buffId;
            res.seatOrWorldId = e.uGameRoom.seatId;
            this.BroardcastOnLook(res);

            return (int)Respones.OK;
        }

        protected abstract int OnPlayerStartSkillInRoom(GM_PlayerEntity e, ReqStartSkill req);

        public int DoPlayerStartSkill(GM_PlayerEntity e, long playerId, ReqStartSkill req)
        {
            int status = this.OnPlayerStartSkillInRoom(e, req);
            if (status != (int)Respones.OK)
            {
                return status;
            }

            // 广播给房间里面所有的人，它释放了一个技能;
            ResStartSkill res = new ResStartSkill();
            res.skillId = req.skillId;
            res.seatOrWorldId = e.uGameRoom.seatId;
            this.BroardcastOnLook(res);

            return (int)Respones.OK;
        }


        /// <summary>
        /// 重置房间数据
        /// </summary>
        public virtual void RestRoomData()
        {
            curTime = 0.0f;
            this.roomState = (int)RoomState.Waiting;
            isCheckGameStarted = false;
            isCheckCaculate = false;
            hasReConnectPlayer = false;
            for (int i = 0; i < playerSeats.Length; i++)
            {
                if (playerSeats[i] != -1)
                {
                    GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerSeats[i]);
                    if (playerEntity != null)
                    {
                        playerEntity.uGameRoom.playerInRoomState = (int)PlayerInRoom.Sit;
                    }
                    if (playerEntity.uGameRoom.reConnectGameState == 2)
                    {
                        playerEntity.uGameRoom.reConnectGameState = -1;
                    }
                    if (playerEntity.uPlayer.session == null)
                    {
                        PlayerExitRoom(playerEntity, playerSeats[i], (int)QuitReason.DisconnectQuit);
                    }
                }
            }
        }
        #endregion



        #region 玩家重连后需要将房间里的数据同步给重连的玩家
        protected abstract void SendReconnectDataToPlayers();
        protected abstract object GetReconnectRoomData(GM_PlayerEntity playerEntity);
        #endregion



        #region 广播发送消息
        //广播消息
        public void BroardcastOnLook(Message m, long exceptPlayerId = -1)
        {
            for (int i = 0; i < onLook.Length; i++)
            {
                if (onLook[i] == -1 || onLook[i] == exceptPlayerId)
                {
                    continue;
                }
                SendMsg(onLook[i], m);
            }
        }
        public void BroardcastInSeat(Message m, long exceptPlayerId = -1)
        {
            for (int i = 0; i < playerSeats.Length; i++)
            {
                if (playerSeats[i] == -1 || playerSeats[i] == exceptPlayerId)
                {
                    continue;
                }
                SendMsg(playerSeats[i], m);
            }
        }


        public void SendMsg(long playerId, Message msg)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uPlayer.session == null)
            {
                return;
            }
            MessagePusher.PushMessage(playerEntity.uPlayer.session, msg, playerId);
        }
        public void SendMsg(GM_PlayerEntity playerEntity, Message msg)
        {
            if (playerEntity == null || playerEntity.uPlayer.session == null)
            {
                return;
            }
            MessagePusher.PushMessage(playerEntity.uPlayer.session, msg, playerEntity.uPlayer.playerInfo.id);
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="req"></param>
        public void SendChatMessage(long playerId, ReqSendChatMessage req)
        {
            if (req.talkType == 0 && req.talkContent.Length <= 0)
            {
                return;
            }

            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                return;
            }
            ResSendChatMessage res = new ResSendChatMessage();
            res.onLookId = playerEntity.uGameRoom.onLookId;
            res.talkType = req.talkType;
            res.talkContent = req.talkContent;
            BroardcastOnLook(res);

        }
        #endregion



        #region 玩家的逻辑
        public bool IsFull()
        {
            return false;
        }

        private int FindEmptySeat()
        {
            for (int i = 0; i < playerSeats.Length; i++)
            {
                if (playerSeats[i] == -1)
                {
                    return i;
                }
            }
            return -1;
        }

        protected abstract int PlayerOperateInRoom(GM_PlayerEntity player, int seatId, ReqPlayerOperation req);

        public ResPlayerOperation PlayerOperation(GM_PlayerEntity player, long playerId, ReqPlayerOperation req)
        {
            ResPlayerOperation res = new ResPlayerOperation();
            if (this.roomState != (int)RoomState.Started)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            int seatId = player.uGameRoom.seatId;
            if (seatId == -1)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }


            int status = this.PlayerOperateInRoom(player, seatId, req);
            if (status == (int)Respones.FrameSyncCache)
            {
                return null;
            }
            else if (status != (int)Respones.OK)
            {
                res.status = status;
                return res;
            }

            res.status = (int)Respones.OK;
            res.operationType = req.operationType;
            res.seatId = seatId;
            res.v1 = req.v1;
            res.v2 = req.v2;
            res.v3 = req.v3;
            this.BroardcastOnLook(res);



            return null;
        }


        //玩家准备
        public int PlayerReady(GM_PlayerEntity playerEntity, long playerId)
        {
            if (this.roomState != (int)RoomState.Waiting ||
                playerEntity.uGameRoom.seatId == -1 ||
                playerEntity.uGameRoom.playerInRoomState != (int)PlayerInRoom.Sit)
            {
                return (int)Respones.InvalidOpt;
            }
            playerEntity.uGameRoom.playerInRoomState = (int)PlayerInRoom.Ready;
            isCheckGameStarted = true;
            ResPlayerReady res = new ResPlayerReady();
            res.status = (int)Respones.OK;
            res.seatId = playerEntity.uGameRoom.seatId;
            BroardcastOnLook(res);
            return (int)Respones.OK;
        }



        /// <summary>
        /// 创建房间内玩家的信息数据
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="seatId"></param>
        /// <returns></returns>
        protected ResUserArrivedSeat CreateUserArrivedData(GM_PlayerEntity playerEntity, int seatId)
        {
            ResUserArrivedSeat userData = new ResUserArrivedSeat();
            userData.unick = playerEntity.uPlayer.playerInfo.name;
            userData.usex = playerEntity.uPlayer.accountInfo.usex;
            userData.uface = playerEntity.uPlayer.accountInfo.uface;
            userData.seatId = seatId;
            userData.playerInRoomState = playerEntity.uGameRoom.playerInRoomState;
            return userData;
        }


        //服务器通知玩家进入房间
        void BroardcastPlayerEnterRoom(GM_PlayerEntity playerEntity)
        {
            ResUserEnterRoom res = new ResUserEnterRoom();
            res.roomId = this.roomId;
            res.roomState = this.roomState;
            res.roomOnLookId = playerEntity.uGameRoom.onLookId;
            for (int i = 0; i < onLook.Length; i++)
            {
                if (onLook[i] == -1)
                {
                    continue;
                }

                SendMsg(onLook[i], res);
            }
        }


        //刚进入房间的玩家 同步其他玩家状态
        void SyncOtherUserState(long playerId)
        {
            for (int i = 0; i < playerSeats.Length; i++)
            {
                if (playerSeats[i] == -1)
                {
                    continue;
                }

                GM_PlayerEntity otherPlayer = GM_EntityMgr.Instance.GetPlayerEntity(playerSeats[i]);
                if (otherPlayer == null)
                {
                    continue;
                }
                ResUserArrivedSeat userData = CreateUserArrivedData(otherPlayer, i);
                SendMsg(playerId, userData);
            }
        }

        /// <summary>
        /// 玩家进入房间
        /// </summary>
        /// <param name="playerId"></param>
        public virtual void PlayerEnterRoom(GM_PlayerEntity playerEntity, long playerId)
        {
            if (!IsFull())
            {

                playerEntity.uGameRoom.roomId = this.roomId;
                playerEntity.uSkillAndBuff.worldOrRoom = this;

                for (int i = 0; i < onLook.Length; i++)
                {
                    if (onLook[i] == -1)
                    {
                        onLook[i] = playerId;
                        playerEntity.uGameRoom.onLookId = i;
                        playerEntity.uGameRoom.playerInRoomState = (int)PlayerInRoom.OnLook;
                        break;
                    }
                }

                BroardcastPlayerEnterRoom(playerEntity);

                SyncOtherUserState(playerId);
            }

        }

        /// <summary>
        /// 玩家重连房间
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public int PlayerReConnectRoom(GM_PlayerEntity playerEntity, long playerId)
        {
            if (playerEntity.uGameRoom.playerInRoomState != (int)PlayerInRoom.Started)
            {
                return (int)Respones.InvalidOpt;
            }
            playerEntity.uPlayer.session.logicServerId = playerEntity.uGameRoom.logicServerId;
            playerEntity.uGameRoom.reConnectGameState = 2;
            hasReConnectPlayer = true;
            return (int)Respones.OK;
        }

        protected abstract void PlayerEscapeFromRoom(GM_PlayerEntity playerEntity, long playrId);

        /// <summary>
        /// 玩家离开房间
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public virtual int PlayerExitRoom(GM_PlayerEntity playerEntity, long playerId, int reason = 0)
        {
            //GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity != null)
            {
                if (playerEntity.uGameRoom.playerInRoomState == (int)PlayerInRoom.Started)
                {
                    if (reason != (int)QuitReason.ForcedQuit)
                    {
                        if (reason == (int)QuitReason.DisconnectQuit)
                        {
                            playerEntity.uGameRoom.reConnectGameState = 1;
                        }
                        return (int)Respones.UserIsPlaying;
                    }
                    if (reason == (int)QuitReason.ForcedQuit)
                    {
                        PlayerEscapeFromRoom(playerEntity, playerId);
                    }
                }
            }


            if (playerEntity.uGameRoom.seatId != -1)
            {
                PlayerStandUp(playerId);
            }


            for (int i = 0; i < onLook.Length; i++)
            {
                if (onLook[i] == playerId)
                {
                    onLook[i] = -1;
                    playerEntity.uGameRoom.onLookId = -1;
                    break;
                }
            }

            playerEntity.uGameRoom.roomId = -1;
            playerEntity.uGameRoom.reConnectGameState = -1;
            playerEntity.uSkillAndBuff.worldOrRoom = null;
            playerEntity.uGameRoom.playerInRoomState = (int)PlayerInRoom.Invalid;
            return (int)Respones.OK;
        }

        /// <summary>
        /// 玩家坐下
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="seatId"></param>
        /// <returns></returns>
        public int PlayerSitDown(GM_PlayerEntity playerEntity, long playerId, int seatId = -1)
        {
            if (this.roomState != (int)RoomState.Waiting)
            {
                return (int)Respones.InvalidOpt;
            }

            int playerSeatId = playerEntity.uGameRoom.seatId;
            if (playerSeatId != -1)
            {
                return (int)Respones.InvalidOpt;
            }

            if (seatId == -1)
            {
                seatId = FindEmptySeat();
            }

            if (seatId < 0 || seatId >= playerSeats.Length || playerSeats[seatId] != -1)
            {
                return (int)Respones.InvalidParams;
            }
            playerSeats[seatId] = playerId;
            playerEntity.uGameRoom.seatId = seatId;
            playerEntity.uGameRoom.playerInRoomState = (int)PlayerInRoom.Sit;

            ResUserArrivedSeat userData = CreateUserArrivedData(playerEntity, seatId);
            BroardcastOnLook(userData, playerId);

            return (int)Respones.OK;
        }

        /// <summary>
        /// 玩家站起
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public int PlayerStandUp(long playerId)
        {
            if (this.roomState == (int)RoomState.Ready || this.roomState == (int)RoomState.Started)
            {
                return (int)Respones.InvalidOpt;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            int playerSeatId = playerEntity.uGameRoom.seatId;
            if (playerEntity == null || playerSeatId == -1)
            {
                return (int)Respones.InvalidOpt;
            }
            playerSeats[playerSeatId] = -1;
            playerEntity.uGameRoom.playerInRoomState = (int)PlayerInRoom.OnLook;

            ResUserExitSeat userExitData = new ResUserExitSeat();
            userExitData.seatId = playerEntity.uGameRoom.seatId;
            playerEntity.uGameRoom.seatId = -1;
            BroardcastOnLook(userExitData, playerId);
            return (int)Respones.OK;
        }

        #endregion        
    }

}

