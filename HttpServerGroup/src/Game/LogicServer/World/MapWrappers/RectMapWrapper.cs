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

        public void PlayerSpawnAtMap(GM_PlayerEntity player, int spawnIndex = -1)
        {
            player.uTransform.pos = new Vector3(GameUtils.Random(-10, 10), 0, GameUtils.Random(-10, 10));
            player.uTransform.eulerAngles = new Vector3(0, 0, 0);
        }


        public void NavToDst(GM_PlayerEntity player, params float[] pos)
        {
            Vector3 dst = new Vector3(pos[0], pos[1], pos[2]);
            Vector3 dir = Vector3.GetDirection(player.uTransform.pos, dst);
            player.uTransform.eulerAngles.y = (MathF.Atan2(dir.z, dir.x) * 180) / MathF.PI;

            float distance = Vector3.GetDistance(ref player.uTransform.pos, ref dst);
            player.uNav.moveTotalTime = distance / player.uProps.speed;
            player.uNav.vx = (dir.x / distance) * player.uProps.speed;
            player.uNav.vz = (dir.z / distance) * player.uProps.speed;
            player.uNav.passedTime = 0;

            player.uStatus.status = (int)CharactorStatus.Run;
        }

        public void NavUpdate(AOIMgr aoiMgr, GM_PlayerEntity player, float dt)
        {
            PointToPointNavSystem.OnUpdate(player, aoiMgr, dt);            
        }

        public int MapWidth()
        {
            return this.mapWidth;
        }

        public int MapHeight()
        {
            return this.mapHeight;
        }
    }
}
