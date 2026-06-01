using Game.Datas.GMEntities;
using System;

namespace Game.LogicServer.RVOMapConfig
{
    [Serializable]
    public class EnmeyConfig
    {
        public int typeId;

        public Vector3 pos;
        public Vector3 rot;

        public FightConfig fConfig = null;
        // ...
    }
}
