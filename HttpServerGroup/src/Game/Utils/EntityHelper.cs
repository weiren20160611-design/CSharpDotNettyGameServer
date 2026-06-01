using Game.Datas.GMEntities;
using Game.Datas.Messages;
using Game.LogicServer;

namespace Game
{
    public class EntityHelper
    {
        public static ResSyncCharactorStatus GetResSyncCharactorStatus(GM_PlayerEntity player)
        {
            ResSyncCharactorStatus res = new ResSyncCharactorStatus();
            res.statusData = new CharactorStatusData();
            res.statusData.aniStatus = player.uStatus.status;
            res.worldId = player.uWorld.worldId;
            res.transform = new CharactorTransform();
            res.transform.pos = new float[3];
            res.transform.pos[0] = player.uTransform.pos.x;
            res.transform.pos[1] = player.uTransform.pos.y;
            res.transform.pos[2] = player.uTransform.pos.z;
            res.transform.eulerAngles = new float[3];
            res.transform.eulerAngles[0] = player.uTransform.eulerAngles.x;
            res.transform.eulerAngles[1] = player.uTransform.eulerAngles.y;
            res.transform.eulerAngles[2] = player.uTransform.eulerAngles.z;
            return res;
        }
        public static void SyncEntityStatus(GM_PlayerEntity player, AOIMgr aoi, int status)
        {
            if (player.uStatus.status == status)
            {
                return;
            }
            player.uStatus.status = status;
            
            ResSyncCharactorStatus res = GetResSyncCharactorStatus(player);


            aoi.BroadMessageToAOI(player, res);
        }
    }

}
