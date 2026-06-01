using Framework.Core.Serializer;
using Framework.Core.task;
using Game.Core.EntityMgr;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System.Collections.Generic;

namespace Game.LogicServer
{
    public class FrameSyncRoomMgr : RoomMgr
    {
        private int frameId;
        private List<ResFrameSyncData> historyFrameOperation = null;
        private List<FrameOperation> nextFrameOperation = null;
        public override void Init(int logicServerTid, int zoneId, BaseLogicServer logicServer = null)
        {
            base.Init(logicServerTid, zoneId, logicServer);
            this.MAX_PLAYER_NUM = 2;
            this.MIN_PLAYER_READY_NUM = 2;
            this.GAME_DURATION = 30 * 60;
            this.READY_TIME = 3;
            this.frameId = -1;
            nextFrameOperation = new List<FrameOperation>();
            historyFrameOperation = new List<ResFrameSyncData>();
        }

        protected override void StartGameUpdate(float dt)
        {
            //这里处理游戏中的逻辑
            this.frameId++;
            ResFrameSyncData res = new ResFrameSyncData();
            res.frameId = this.frameId;
            res.frameOperationSet = nextFrameOperation.ToArray();
            this.BroardcastOnLook(res);
            //end
            this.nextFrameOperation.Clear();
            //this.historyFrameOperation.Add(res);

            curTime += dt;
            if (curTime >= GAME_DURATION)
            {
                GameEnterCheckOutStage();
            }
        }
        public override void RestRoomData()
        {
            base.RestRoomData();
            this.frameId = -1;
            this.historyFrameOperation.Clear();
            this.nextFrameOperation.Clear();
        }

        protected override void GameCheckOutStage()
        {
            ResCheckOutGame res = new ResCheckOutGame();
            res.reserve = 0;
            BroardcastOnLook(res);
            RestRoomData();
        }
        protected override void PlayerEscapeFromRoom(GM_PlayerEntity playerEntity, long playrId)
        {
            ResPlayerEscape res = new ResPlayerEscape();
            res.seatId = playerEntity.uGameRoom.seatId;
            res.unick = playerEntity.uPlayer.playerInfo.name;
            BroardcastOnLook(res, playrId);

            GameEnterCheckOutStage();

        }



        /// <summary>
        /// 获取房间内的玩家数据
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 发送房间内的玩家数据给断线重连的玩家
        /// </summary>
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

        protected override int PlayerOperateInRoom(GM_PlayerEntity player, int seatId, ReqPlayerOperation req)
        {
            int frameId = req.v3;
            if (frameId != this.frameId + 1)
            {
                return (int)Respones.InvalidParams;
            }
            FrameOperation operation = new FrameOperation();
            operation.operationType = req.operationType;
            operation.seatId = seatId;
            operation.v1 = req.v1;
            operation.v2 = req.v2;
            this.nextFrameOperation.Add(operation);

            return (int)Respones.FrameSyncCache;
        }

        protected override int OnPlayerStartBuffInRoom(GM_PlayerEntity e, ReqStartBuff req)
        {
            return (int)Respones.OK;
        }

        protected override int OnPlayerStartSkillInRoom(GM_PlayerEntity e, ReqStartSkill req)
        {
            return (int)Respones.OK;
        }
    }
}
