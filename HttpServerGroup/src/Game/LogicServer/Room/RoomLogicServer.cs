using Framework.Core.Net;
using Framework.Core.Utils;
using Game.Core.EntityMgr;
using Game.Datas.DBEntities;
using Game.Datas.Excels;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using Game.Utils;
using System.Collections.Generic;


namespace Game.LogicServer
{
    [LogicServerMeta((int)ServerType.Logic1)]
    public partial class RoomLogicServer : CommonLogicServer
    {

        //private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        //分区ID->玩家等待列表
        private Dictionary<int, List<long>> zoneId_PlayerWaitList = null;

        //分区ID->房间ID->房间
        private Dictionary<int, Dictionary<int, RoomMgr>> zoneId_RoomId_Room = null;
        //房间ID->房间
        private Dictionary<int, RoomMgr> roomId_Room = null;




        public override void OnStart()
        {
            base.OnStart();
            LogicServerInstance config = this.instanceConfig as LogicServerInstance;
            this.logger.Info($"逻辑服{this.logicServerId}:{config.desic}开启");
            zoneId_PlayerWaitList = new Dictionary<int, List<long>>();
            zoneId_RoomId_Room = new Dictionary<int, Dictionary<int, RoomMgr>>();
            roomId_Room = new Dictionary<int, RoomMgr>();
            foreach (var zoneId in zoneId_ZoneConfig.Keys)
            {
                List<long> waitList = new List<long>();
                zoneId_PlayerWaitList.Add(zoneId, waitList);
                zoneId_RoomId_Room.Add(zoneId, new Dictionary<int, RoomMgr>());
            }

        }

        public void OnUpdate(float dt)
        {
            MatchPlayerToRoom();
            foreach (var roomSet in zoneId_RoomId_Room.Values)
            {
                foreach (var room in roomSet.Values)
                {
                    room.OnUpdate(dt);
                }
            }
        }



        private RoomMgr GetOrCreateRoom(int zoneId)
        {
            Dictionary<int, RoomMgr> roomDic = null;
            if (!zoneId_RoomId_Room.ContainsKey(zoneId))
            {
                roomDic = new Dictionary<int, RoomMgr>();
                zoneId_RoomId_Room.Add(zoneId, roomDic);
            }

            roomDic = zoneId_RoomId_Room[zoneId];
            foreach (var room in roomDic.Values)
            {
                if (room.roomState == (int)RoomState.Waiting)
                {
                    return room;
                }
            }

            RoomMgr roomMgr = RoomFactory.CreateRoom(this.logicServerTid, zoneId);
            roomMgr.Init(logicServerTid, zoneId, this);
            roomDic.Add(roomMgr.roomId, roomMgr);
            roomId_Room.Add(roomMgr.roomId, roomMgr);
            return roomMgr;
        }

        protected RoomMgr GetRoomByRoomId(int roomId)
        {
            if (roomId_Room.ContainsKey(roomId))
            {
                return roomId_Room[roomId];
            }
            return null;
        }

        private void MatchPlayerToRoom()
        {
            foreach (var zoneId in zoneId_PlayerWaitList.Keys)
            {
                List<long> zoneIdWaitList = zoneId_PlayerWaitList[zoneId];
                for (int i = 0; i < zoneIdWaitList.Count; i++)
                {
                    RoomMgr roomMgr = GetOrCreateRoom(zoneId);
                    long playerId = zoneIdWaitList[i];
                    GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
                    if (playerEntity == null)
                    {
                        continue;
                    }
                    roomMgr.PlayerEnterRoom(playerEntity, playerId);
                }
                zoneIdWaitList.Clear();
            }
        }
        /// <summary>
        /// 进入逻辑服
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="zoneId"></param>
        /// <param name="logicServerId"></param>
        /// <returns></returns>
        public override int EnterLogicServer(long playerId, int zoneId, int logicServerId)
        {
            if (!this.zoneId_ZoneConfig.ContainsKey(zoneId))
            {
                return (int)Respones.InvalidParams;
            }

            LogicServerInstance logicServerInstance = this.instanceConfig as LogicServerInstance;
            if (logicServerInstance != null)
            {
                if (this.onelinePlayers >= logicServerInstance.maxUser)
                {
                    return (int)Respones.LogicServerIsBusy;
                }
            }
            this.onelinePlayers++;

            ZoneConfig zoneConfig = ExcelUtils.GetConfigData<ZoneConfig>(zoneId.ToString());
            if (zoneConfig == null)
            {
                return (int)Respones.InvalidParams;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                return (int)Respones.InvalidOpt;
            }
            if (playerEntity.uGameRoom.zoneId >= 0 || playerEntity.uGameRoom.roomId >= 0)
            {
                return (int)Respones.InvalidOpt;
            }
            int status = ZoneRule.PlayerCanEnterZone(zoneConfig.condString, playerEntity);
            if (status != (int)Respones.OK)
            {
                return status;
            }


            playerEntity.uGameRoom.roomId = -1;
            playerEntity.uGameRoom.zoneId = zoneId;
            playerEntity.uGameRoom.logicServerId = logicServerId;
            List<long> waitList = zoneId_PlayerWaitList[zoneId];
            waitList.Add(playerId);


            return (int)Respones.OK;
        }
        public override int ReConnectGameInLogicServer(long playerId)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                return (int)Respones.InvalidOpt;
            }
            RoomMgr room = GetRoomByRoomId(playerEntity.uGameRoom.roomId);
            if (room == null)
            {
                return (int)Respones.InvalidOpt;
            }
            return room.PlayerReConnectRoom(playerEntity, playerId);
        }

        public override int QuitLogicServer(long playerId, int reason)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                return (int)Respones.InvalidOpt;
            }

            if (reason == (int)QuitReason.DisconnectQuit)
            {
                playerEntity.uPlayer.session = null;
            }

            int status = 0;
            if (playerEntity.uGameRoom.zoneId != -1 && playerEntity.uGameRoom.roomId != -1)
            {
                RoomMgr room = GetRoomByRoomId(playerEntity.uGameRoom.roomId);
                if (room != null)
                {
                    status = room.PlayerExitRoom(playerEntity, playerId, reason);
                    if (status != (int)Respones.OK)
                    {
                        return status;
                    }
                }

            }
            this.onelinePlayers--;
            playerEntity.uGameRoom.zoneId = -1;
            playerEntity.uGameRoom.roomId = -1;
            playerEntity.uGameRoom.logicServerId = -1;
            return status;
        }
    }
}
