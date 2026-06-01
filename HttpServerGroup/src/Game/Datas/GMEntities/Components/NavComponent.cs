using Game.Core.Db;
using Game.Core.GM_Backpack;
using Game.Datas.DBEntities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game.Datas.GMEntities
{
    public struct NavComponent
    {
        public float moveTotalTime;
        public float vx;
        public float vz;
        public float passedTime;

        public void Init(GM_PlayerEntity entity)
        {
            entity.uNav.moveTotalTime = 0.0f;
            entity.uNav.vx = 0.0f;
            entity.uNav.vz = 0.0f;
        }
    }
}
