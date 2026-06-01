using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Datas.GMEntities
{
    public struct GameRoomComponent
    {
        public int zoneId;
        public int roomId;
        public int seatId;
        public int onLookId;
        public int logicServerId;
        public int playerInRoomState;
        public int reConnectGameState;
        public static void Init(GM_PlayerEntity entity)
        {
            entity.uGameRoom.zoneId = -1;
            entity.uGameRoom.roomId = -1;
            entity.uGameRoom.seatId = -1;
            entity.uGameRoom.onLookId = -1;
            entity.uGameRoom.logicServerId = -1;
            entity.uGameRoom.playerInRoomState = -1;
            entity.uGameRoom.reConnectGameState = -1;
        }

        public static void Reset(GM_PlayerEntity entity)
        {
            entity.uGameRoom.zoneId = -1;
            entity.uGameRoom.roomId = -1;
            entity.uGameRoom.seatId = -1;
            entity.uGameRoom.onLookId = -1;
            entity.uGameRoom.logicServerId = -1;
            entity.uGameRoom.playerInRoomState = -1;
            entity.uGameRoom.reConnectGameState = -1;
        }

        public static void Exit(GM_PlayerEntity playerEntity)
        {

        }
    }
}
