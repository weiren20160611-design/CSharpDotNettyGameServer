using Game.Datas.GMEntities;

namespace Game.LogicServer
{
    public class PointToPointNavSystem
    {
        public static void OnUpdate(GM_PlayerEntity player, AOIMgr aoiMgr, float dt)
        {
            player.uNav.passedTime += dt;
            if (player.uNav.passedTime > player.uNav.moveTotalTime)
            {
                dt -= (player.uNav.passedTime - player.uNav.moveTotalTime);
            }
            player.uTransform.pos.x += player.uNav.vx * dt;
            player.uTransform.pos.z += player.uNav.vz * dt;

            if (player.uNav.passedTime > player.uNav.moveTotalTime)
            {
                EntityHelper.SyncEntityStatus(player, aoiMgr, (int)CharactorStatus.Idle);
            }
        }
    }
}
