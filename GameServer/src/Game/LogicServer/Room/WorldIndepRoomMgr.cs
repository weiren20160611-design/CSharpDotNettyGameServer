using Framework.Core.task;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.LogicServer
{
    public class WorldIndepRoomMgr : RoomMgr
    {
        public GameWorld gameWorld = null;
        public override void Init(int logicServerTid, int zoneId, BaseLogicServer logicServer = null)
        {
            this.MAX_PLAYER_NUM = 2;
            this.ON_LOOK_NUM = 8;
            this.MIN_PLAYER_READY_NUM = 2;
            this.GAME_DURATION = -1;//游戏结束条件决出胜利的一方为止
            base.Init(logicServerTid, zoneId, logicServer);
            this.gameWorld = new GameWorld();
            this.gameWorld.Init(zoneId / 10, this.timerMgr);
        }

        public override void PlayerEnterRoom(GM_PlayerEntity playerEntity, long playerId)
        {
            base.PlayerEnterRoom(playerEntity, playerId);
            this.roomState = (int)RoomState.Started;

            this.gameWorld.PlayerEnterToWorld(playerEntity, playerId);
        }

        public override int PlayerExitRoom(GM_PlayerEntity playerEntity, long playerId, int reason = 0)
        {
            this.gameWorld.PlayerExitFromWorld(playerEntity, playerId, reason);
            if (reason == (int)QuitReason.DisconnectQuit)
            {
                reason = (int)QuitReason.ForcedQuit;
            }

            this.roomState = (int)RoomState.Waiting;

            return base.PlayerExitRoom(playerEntity, playerId, reason);
        }


        protected override void GameCheckOutStage()
        {
            
        }

        protected override object GetReconnectRoomData(GM_PlayerEntity playerEntity)
        {
            return null;
        }

        protected override int OnPlayerStartBuffInRoom(GM_PlayerEntity e, ReqStartBuff req)
        {
            return (int)Respones.OK;
        }

        protected override int OnPlayerStartSkillInRoom(GM_PlayerEntity e, ReqStartSkill req)
        {
            return (int)Respones.OK;
        }

        protected override void PlayerEscapeFromRoom(GM_PlayerEntity playerEntity, long playrId)
        {
            ResPlayerEscape res = new ResPlayerEscape();
            res.seatId = playerEntity.uGameRoom.seatId;
            res.unick = playerEntity.uPlayer.playerInfo.name;
            BroardcastOnLook(res, playrId);

            GameEnterCheckOutStage();
        }

        protected override int PlayerOperateInRoom(GM_PlayerEntity player, int seatId, ReqPlayerOperation req)
        {
            return (int)Respones.OK;
        }

        protected override void SendReconnectDataToPlayers()
        {
            
        }

        protected override void StartGameUpdate(float dt)
        {
            this.gameWorld.OnUpdate(dt);
        }
    }
}
