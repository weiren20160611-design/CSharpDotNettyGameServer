using Game.Datas.GMEntities;
using System;
using System.Collections.Generic;

namespace Game.LogicServer
{
    public class AStarNavSystem
    {
        public static void StartRoadNavAction(BaseEntity player, List<RoadNode> roadNodes)
        {
            ref AStarComponent aStar = ref player.uAStar;
            //aStar.isMove = true;
            aStar.moveTime = aStar.passedTime = 0;
            aStar.roadNodeArray = roadNodes;
            aStar.nextIndex = 0;
        }

        public static void StopNavAction(AOIMgr aoi, BaseEntity e)
        {
            ref AStarComponent nav = ref e.uAStar;
            nav.roadNodeArray = null;
            //nav.isMove = false;
            EntityHelper.SyncEntityStatus(e, aoi, (int)CharactorStatus.Idle);
        }


        private static void WalkToNext(BaseEntity e)
        {
            ref AStarComponent nav = ref e.uAStar;


            Vector2 startPos = new Vector2(e.uTransform.pos.x, e.uTransform.pos.z);
            Vector2 endPos = new Vector2(nav.roadNodeArray[nav.nextIndex].px, nav.roadNodeArray[nav.nextIndex].py);

            Vector2 dir = Vector2.Sub(ref endPos, ref startPos);
            float len = Vector2.Len(ref dir);
            nav.moveTime = len / e.uProps.speed; // 可能后续要叠加我们的buff, 结合当前的Buff系统，来计算玩家的最终移动速度;
            nav.passedTime = 0;

            nav.vx = dir.x * e.uProps.speed / len;
            nav.vz = dir.y * e.uProps.speed / len;

            // 同步一下我们角色的方向, 角度为"度";
            e.uTransform.eulerAngles.y = (MathF.Atan2(dir.y, dir.x) * 180) / MathF.PI;

        }

        public static void NavRoadUpdate(BaseEntity e, AOIMgr aoi, float dt)
        {
            ref AStarComponent nav = ref e.uAStar;

            if (nav.passedTime >= nav.moveTime)
            {
                nav.nextIndex++;
                if (nav.nextIndex >= nav.roadNodeArray.Count)
                { //  行走到了目的地
                    AStarNavSystem.StopNavAction(aoi, e);
                    return;
                }

                AStarNavSystem.WalkToNext(e);
            }

            nav.passedTime += dt;
            if (nav.passedTime > nav.moveTime)
            {
                dt -= (nav.passedTime - nav.moveTime);
            }

            e.uTransform.pos.x += (nav.vx * dt);
            e.uTransform.pos.z += (nav.vz * dt);
        }
    }
}
