using Framework.Core.Net;
using Game.Datas.DBEntities;

namespace Game.Datas.GMEntities
{
    public struct NineGridComponent
    {
        public int gridX;
        public int gridY;

        public float dstPosX;
        public float dstPosY;
        public float dstPosZ;

        public void Init(GM_PlayerEntity entity)
        {
            entity.uNineGrid.gridX = entity.uNineGrid.gridY = -1;
            entity.uNineGrid.dstPosX = -1;
            entity.uNineGrid.dstPosY = -1;
            entity.uNineGrid.dstPosZ = -1;
        }
    }
}