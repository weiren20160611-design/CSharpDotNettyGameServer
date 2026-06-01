using Game.Datas.GMEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.LogicServer
{
    public interface IMapWrapper
    {
        public void LoadMapData(int mapId);

        public void PlayerSpawnAtMap(GM_PlayerEntity player, int spawnIndex = -1);

        public void NavToDst(GM_PlayerEntity player, params float[] dst);

        public void NavUpdate(AOIMgr aoiMgr, GM_PlayerEntity player, float dt);

        public int MapWidth();

        public int MapHeight();
    }
}
