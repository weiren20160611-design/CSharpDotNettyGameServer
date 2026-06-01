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
        public void NavUpdate(AOIMgr aoiMgr, BaseEntity player, float dt);

        public void NavLateUpdate(AOIMgr aoiMgr, BaseEntity player, float dt);

        public void LoadMapData(int mapId);

        public int MapWidth();

        public int MapHeight();

        public void PlayerSpawnAtMap(BaseEntity player, int spawnIndex = -1);

        public void NavToDst(BaseEntity player, AOIMgr aoi, float x, float y, float z);

        public void NavToDir(BaseEntity player, AOIMgr aoi, int x, int y);



    }
}
