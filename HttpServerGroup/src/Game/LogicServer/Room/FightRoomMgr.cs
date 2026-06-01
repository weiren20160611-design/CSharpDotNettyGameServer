using Framework.Core.Serializer;
using Framework.Core.task;
using Game.Core.EntityMgr;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System.Collections.Generic;

namespace Game.LogicServer
{
    public class FightRoomMgr : RoomMgr
    {

        public override void Init(int logicServerTid, int zoneId, BaseLogicServer logicServer = null)
        {
            this.MAX_PLAYER_NUM = 2;
            this.ON_LOOK_NUM = 8;
            this.MIN_PLAYER_READY_NUM = 2;
            this.GAME_DURATION = -1;//游戏结束条件决出胜利的一方为止
            base.Init(logicServerTid, zoneId, logicServer);
        }

        private void SkillAndBuffUpdate(float dt)
        {
            for (int i = 0; i < playerSeats.Length; i++)
            {
                if (playerSeats[i] == -1)
                {
                    continue;
                }
                GM_PlayerEntity player = GM_EntityMgr.Instance.GetPlayerEntity(playerSeats[i]);
                SkillAndBuffSystem.Update(this, player, dt);
            }
        }

        protected override void StartGameUpdate(float dt)
        {
            this.SkillAndBuffUpdate(dt);
            //这里处理游戏中的逻辑
            //end
            //curTime += dt;
            //if (curTime >= GAME_DURATION)
            //{
            //    GameEnterCheckOutStage();
            //}
        }


        protected override void GameCheckOutStage()
        {
            // 对游戏进行结算,测试代码，具体可以根据自己的游戏来定制;
            int winnerId = -1;

            for (int i = 0; i < this.playerSeats.Length; i++)
            {
                if (this.playerSeats[i] == -1)
                {
                    continue;
                }

                GM_PlayerEntity e = GM_EntityMgr.Instance.GetPlayerEntity(this.playerSeats[i]);
                if (e == null)
                {
                    continue;
                }

                if (e.uStatus.status != (int)CharactorStatus.Die)
                {
                    winnerId = e.uGameRoom.seatId;
                }
            }
            // end

            // 把游戏的结果和游戏的数据结算出来，发往客户端
            ResCheckOutGame res = new ResCheckOutGame();
            res.reserve = winnerId; // -1 为平局，
            BroardcastOnLook(res);
        }
        protected override void PlayerEscapeFromRoom(GM_PlayerEntity playerEntity, long playrId)
        {
            ResPlayerEscape res = new ResPlayerEscape();
            res.seatId = playerEntity.uGameRoom.seatId;
            res.unick = playerEntity.uPlayer.playerInfo.name;
            BroardcastOnLook(res, playrId);

            GameEnterCheckOutStage();

        }


        protected override ResReConnectSyncData GetReconnectRoomData(GM_PlayerEntity playerEntity)
        {
            if (this.roomState != (int)RoomState.Started && this.roomState != (int)RoomState.Ready)
            {
                return null;
            }
            List<ResUserArrivedSeat> listData = new List<ResUserArrivedSeat>();
            for (int i = 0; i < playerSeats.Length; i++)
            {
                if (playerSeats[i] == -1 || i == playerEntity.uGameRoom.seatId)
                {
                    continue;
                }

                GM_PlayerEntity player = GM_EntityMgr.Instance.GetPlayerEntity(playerSeats[i]);
                ResUserArrivedSeat data = CreateUserArrivedData(player, i);
                listData.Add(data);
            }
            ResReConnectSyncData res = new ResReConnectSyncData();
            res.userArrivedDatas = listData.ToArray();
            return res;
        }

        protected override void SendReconnectDataToPlayers()
        {
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
                if (playerEntity.uGameRoom.reConnectGameState == 2)
                {
                    ResReConnectSyncData datas = GetReconnectRoomData(playerEntity);
                    if (datas != null)
                    {
                        datas.selfInRoomInfo = new SelfInRoomInfo();
                        playerEntity.uGameRoom.reConnectGameState = -1;
                        datas.selfInRoomInfo.roomState = this.roomState;
                        datas.selfInRoomInfo.roomId = this.roomId;
                        datas.selfInRoomInfo.selfSeatId = playerEntity.uGameRoom.seatId;
                        datas.selfInRoomInfo.roomOnLookId = playerEntity.uGameRoom.onLookId;
                        SendMsg(playerEntity, datas);
                    }

                }
            }
        }


        public override void PlayerEnterRoom(GM_PlayerEntity player,long playerId)
        {
            base.PlayerEnterRoom(player,playerId);
           
            int status = base.PlayerSitDown(player, playerId);
            ResSitdown res = new ResSitdown();
            res.status = status;
            res.seatId = player.uGameRoom.seatId;
            this.SendMsg(player, res);

            player.uProps.hp = 500;
            player.uStatus.status = (int)CharactorStatus.Idle;

            base.PlayerReady(player, playerId);
        }

        /// <summary>
        /// 玩家离开房间
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public override int PlayerExitRoom(GM_PlayerEntity playerEntity, long playerId, int reason = 0)
        {
            if (reason == (int)QuitReason.DisconnectQuit)
            {
                reason = (int)QuitReason.ForcedQuit;
            }

            return base.PlayerExitRoom(playerEntity, playerId, reason);
        }

        protected override void PlayerOperateInRoom(GM_PlayerEntity player, int seatId, ReqPlayerOperation req)
        {
            throw new System.NotImplementedException();
        }

        protected override int OnPlayerStartSkillInRoom(GM_PlayerEntity e, ReqStartSkill req)
        {
            if (this.roomState != (int)RoomState.Started || e.uStatus.status == (int)CharactorStatus.Invalid ||
                e.uStatus.status == (int)CharactorStatus.Die)
            {
                return (int)Respones.InvalidOpt;
            }

            if (!e.uSkillAndBuff.skillTimeLine.StartSkill(e, req.skillId, null))
            {
                return (int)Respones.InvalidOpt;
            }

            return (int)Respones.OK;
            // return (int)Respones.InvalidOpt;
        }

        protected override int OnPlayerStartBuffInRoom(GM_PlayerEntity e, ReqStartBuff req)
        {
            if (this.roomState != (int)RoomState.Started)
            {
                return (int)Respones.InvalidOpt;
            }

            if (!e.uSkillAndBuff.buffTimeLine.StartBuff(req.buffId))
            {
                return (int)Respones.InvalidOpt;
            }

            return (int)Respones.OK;
        }

        // 暂时测试
        public GM_PlayerEntity[] FindEntitiesInArea(GM_PlayerEntity e, float attackR)
        {
            return null;
        }

        public GM_PlayerEntity FindNearestInArea(GM_PlayerEntity e, float attackR)
        {
            if (e.uGameRoom.seatId == 0)
            {
                return GM_EntityMgr.Instance.GetPlayerEntity(this.playerSeats[1]);
            }
            return GM_EntityMgr.Instance.GetPlayerEntity(this.playerSeats[0]);
        }

        public void BroadMsgToAOI(GM_PlayerEntity e, Message s, long ignorePlayerId = -1)
        {
            for (int i = 0; i < this.playerSeats.Length; i++)
            {
                long playerId = this.playerSeats[i];
                if (playerId == -1 || playerId == ignorePlayerId)
                {
                    continue;
                }

                this.SendMsg(playerId, s);
            }
        }

        public void OnPlayerEntityLostHp(GM_PlayerEntity e, int lost)
        {
            e.uProps.hp -= lost;

            ResLostHp res = new ResLostHp();
            res.lostHp = lost;
            res.seatOrWorldId = e.uGameRoom.seatId;
            this.BroadMsgToAOI(e, res);            

            if (e.uProps.hp <= 0)
            {
                this.OnPlayerEntityDied(e);
            }
        }


        private void OnPlayerEntityDied(GM_PlayerEntity e)
        {
            e.uStatus.status = (int)CharactorStatus.Die;
            ResSyncCharactorStatus res = EntityHelper.GetResSyncCharactorStatus(e);
            res.worldId = e.uGameRoom.seatId; // 注意一下，房间模式的id为seatId;
            this.BroadMsgToAOI(e, res);

            // 游戏直接结束，进入结算
            this.GameEnterCheckOutStage();
            // end
        }
    }
}
