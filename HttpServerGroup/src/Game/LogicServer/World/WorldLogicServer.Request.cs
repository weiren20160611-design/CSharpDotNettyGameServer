using Framework.Core.Net;
using Framework.Core.Utils;
using Game.Core.EntityMgr;
using Game.Datas.DBEntities;
using Game.Datas.GMEntities;
using Game.Datas.Messages;

namespace Game.LogicServer
{

    public partial class WorldLogicServer
    {

        [RequestMapping]
        public object DoReqPlayerSpawn(IdSession s, ReqPlayerSpawn req)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(s.playerID);
            if (playerEntity == null || playerEntity.uWorld.worldId == -1)
            {
                ResPlayerSpawn res = new ResPlayerSpawn();
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            return this.gameWorld.PlayerSpawnAt(playerEntity, req);
        }

        [RequestMapping]
        public object DoReqNavToDst(IdSession s, ReqNavToDst req)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(s.playerID);
            if (playerEntity == null || playerEntity.uWorld.worldId == -1)
            {
                return null;
            }

            this.gameWorld.PlayerNavToDst(playerEntity, req);
            return null;
        }

    }
}
