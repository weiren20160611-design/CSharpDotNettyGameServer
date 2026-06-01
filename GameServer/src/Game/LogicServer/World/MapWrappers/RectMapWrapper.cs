using Game.Datas.GMEntities;
using Game.Utils;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;


namespace Game.LogicServer
{
    public class RectMapWrapper : IMapWrapper
    {
        private int mapId;
        private int mapWidth;
        private int mapHeight;

        public void LoadMapData(int mapId)
        {
            this.mapId = mapId;
            this.mapWidth = 50;
            this.mapHeight = 50;
        }

        public void NavUpdate(AOIMgr aoiMgr, BaseEntity player, float dt)
        {
            PointToPointNavSystem.OnUpdate(player, aoiMgr, dt);            
        }
        public void NavLateUpdate(AOIMgr aoiMgr, BaseEntity player, float dt)
        {

        }
        
        public void PlayerSpawnAtMap(BaseEntity player, int spawnIndex = -1)
        {
            player.uTransform.pos = new Vector3(GameUtils.Random(-10, 10), 0, GameUtils.Random(-10, 10));
            player.uTransform.eulerAngles = new Vector3(0, 0, 0);
        }


        public void NavToDst(BaseEntity player, AOIMgr aoi, float x, float y, float z)
        {
            Vector3 dst = new Vector3(x, y, z);
            Vector3 dir = Vector3.GetDirection(ref player.uTransform.pos, ref dst);
            player.uTransform.eulerAngles.y = (MathF.Atan2(dir.z, dir.x) * 180) / MathF.PI;

            float distance = Vector3.GetDistance(ref player.uTransform.pos, ref dst);
            player.uNav.moveTotalTime = distance / player.uProps.speed;
            player.uNav.vx = (dir.x / distance) * player.uProps.speed;
            player.uNav.vz = (dir.z / distance) * player.uProps.speed;
            player.uNav.passedTime = 0;

            player.uStatus.status = (int)CharactorStatus.Run;
        }

        public int MapWidth()
        {
            return this.mapWidth;
        }

        public int MapHeight()
        {
            return this.mapHeight;
        }

        public void NavToDir(BaseEntity player, AOIMgr aoi, int x, int y)
        {
            
        }
    }
}
