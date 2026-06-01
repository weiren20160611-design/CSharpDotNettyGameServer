using Framework.Core.Serializer;
using Framework.Core.task;
using Game.Core.EntityMgr;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System.Collections.Generic;

namespace Game.LogicServer
{
    public class RoundRoomMgr : RoomMgr
    {
        public override void Init(int logicServerTid, int zoneId, BaseLogicServer logicServer = null)
        {

            base.Init(logicServerTid, zoneId, logicServer);
        }

        protected override void StartGameUpdate(float dt)
        {
            //这里处理游戏中的逻辑
            //end
            curTime += dt;
            if (curTime >= GAME_DURATION)
            {
                GameEnterCheckOutStage();
            }
        }


        protected override void GameCheckOutStage()
        {
            ResCheckOutGame res = new ResCheckOutGame();
            res.reserve = 0;
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
            return (int)Respones.OK;
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
