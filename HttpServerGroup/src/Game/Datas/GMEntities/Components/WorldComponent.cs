using Game.Core.Db;
using Game.Core.GM_Backpack;
using Game.Datas.DBEntities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game.Datas.GMEntities
{
    public struct WorldComponent
    {
        public int zoneId;
        public int worldId;
        public int logicServerId;
        public int playerInWorldStage;
        public int reConnectGameState;
        public static void Init(GM_PlayerEntity entity)
        {
            entity.uWorld.zoneId = -1;
            entity.uWorld.worldId = -1;
            entity.uWorld.logicServerId = -1;
            entity.uWorld.reConnectGameState = -1;
            entity.uWorld.playerInWorldStage = -1;
        }
    }
}
