using Framework.Core.Net;
using Framework.Core.Utils;
using Game.Core.EntityMgr;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.LogicServer
{
    public partial class WorldIndepLogicServer : RoomLogicServer
    {
        [RequestMapping]
        public object DoReqPlayerSpawn(IdSession s, ReqPlayerSpawn req, long playerId)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                ResPlayerSpawn res = new ResPlayerSpawn();
                res.status = (int)Respones.InvalidOpt;
                return res;
            }

            WorldIndepRoomMgr worldRoom = this.GetRoomByRoomId(playerEntity.uGameRoom.roomId) as WorldIndepRoomMgr;

            return worldRoom.gameWorld.PlayerSpawnAt(playerEntity, req);
        }

        [RequestMapping]
        public object DoReqNavToDst(IdSession s, ReqNavToDst req, long playerId)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                return null;
            }
            WorldIndepRoomMgr worldRoom = this.GetRoomByRoomId(playerEntity.uGameRoom.roomId) as WorldIndepRoomMgr;
            worldRoom.gameWorld.PlayerNavToDst(playerEntity, req);
            return null;
        }

        [RequestMapping]
        public object DoReqNavToDir(IdSession s, ReqNavToDir req, long playerId)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                return null;
            }
            WorldIndepRoomMgr worldRoom = this.GetRoomByRoomId(playerEntity.uGameRoom.roomId) as WorldIndepRoomMgr;
            worldRoom.gameWorld.PlayerNavToDir(playerEntity, req);
            return null;
        }
    }
}
