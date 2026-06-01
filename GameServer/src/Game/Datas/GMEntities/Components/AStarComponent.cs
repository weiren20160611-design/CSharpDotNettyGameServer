using Game.Core.Db;
using Game.Core.GM_Backpack;
using Game.Datas.DBEntities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game.Datas.GMEntities
{
    public struct AStarComponent
    {
        public List<RoadNode> roadNodeArray;

        public float moveTime;
        public float passedTime;

        public float vx;
        public float vz;

        //public bool isMove;
        public int nextIndex;

        public static void Init(GM_PlayerEntity entity)
        {
            entity.uAStar.roadNodeArray = null;
            entity.uAStar.moveTime = entity.uAStar.passedTime = 0;
            entity.uAStar.vx = entity.uAStar.vz = 0;
            entity.uAStar.nextIndex = -1;
            //entity.uAStar.isMove = false;
        }

        public static void Reset(GM_PlayerEntity entity)
        {
            entity.uAStar.roadNodeArray = null;
            entity.uAStar.moveTime = entity.uAStar.passedTime = 0;
            entity.uAStar.vx = entity.uAStar.vz = 0;
            entity.uAStar.nextIndex = -1;
            //entity.uAStar.isMove = false;
        }

        public static void Exit(GM_PlayerEntity playerEntity)
        {

        }


    }
}
