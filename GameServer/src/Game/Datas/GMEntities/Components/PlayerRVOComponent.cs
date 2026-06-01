using Framework.Core.Net;
using Game.Datas.DBEntities;

namespace Game.Datas.GMEntities
{
    public struct PlayerRVOComponent
    {
        public int agentId;
        public Vector3 targetPos;

        public float dirX;
        public float dirY;

        public int navType;

        public static void Init(GM_PlayerEntity entity)
        {
            entity.uRVO.agentId = -1;
            entity.uRVO.dirX = 0.0f;
            entity.uRVO.dirY = 0.0f;
            entity.uRVO.navType = -1;
        }

        public static void Reset(GM_PlayerEntity entity)
        {
            entity.uRVO.agentId = -1;
            entity.uRVO.dirX = 0.0f;
            entity.uRVO.dirY = 0.0f;
            entity.uRVO.navType = -1;
        }

        public static void Exit(GM_PlayerEntity playerEntity)
        {

        }
    }
}