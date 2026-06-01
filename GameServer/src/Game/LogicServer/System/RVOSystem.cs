using Game.Datas.DBEntities;
using Game.Datas.GMEntities;
using RVO;
using System;
using System.Collections.Generic;

namespace Game.LogicServer
{
    public class RVOSystem
    {
        public static void StartRvoAction(BaseEntity e, float x, float y, float z)
        {
            e.uStatus.status = (int)CharactorStatus.Run;
            e.uRVO.targetPos = new Vector3(x, y, z);
            e.uRVO.navType = 0;
        }

        public static void StartRvoAction(Simulator simulator, AOIMgr aoi, BaseEntity e, int x, int y)
        {
            if (x == 0 && y == 0)
            {
                e.uRVO.navType = -1;
                SetAgentPrefVelocity(simulator, e.uRVO.agentId, new Vector3(0, 0, 0));
                EntityHelper.SyncEntityStatus(e, aoi, (int)CharactorStatus.Idle);
                return;
            }
            e.uStatus.status = (int)CharactorStatus.Run;
            e.uRVO.navType = 1;
            e.uRVO.dirX = (float)x / (1 << 16);
            e.uRVO.dirY = (float)y / (1 << 16);
            float len = MathF.Sqrt(e.uRVO.dirX * e.uRVO.dirX + e.uRVO.dirY * e.uRVO.dirY);

            e.uRVO.dirX = e.uRVO.dirX / len;
            e.uRVO.dirY = e.uRVO.dirY / len;
        }

        public static void StopRvoAction(BaseEntity player)
        {

        }

        public static void RvoUpdate(Simulator simulator, AOIMgr aoi, BaseEntity e, float dt)
        {
            Vector3 v;
            if (e.uRVO.navType == 1)
            {
                v.x = e.uRVO.dirX * e.uProps.speed;
                v.z = e.uRVO.dirY * e.uProps.speed;
                v.y = 0;
                SetAgentPrefVelocity(simulator, e.uRVO.agentId, v);               
                return;
            }

            Vector3 pos = GetAgentPosition(simulator, e.uRVO.agentId);
            Vector3 dir = Vector3.Sub(ref e.uRVO.targetPos, ref pos);
            float len = Vector3.Len(ref dir);

            if (len <= 0.3f)
            {
                e.uRVO.navType = -1;
                SetAgentPosition(simulator, e.uRVO.agentId, e.uRVO.targetPos);
                SetAgentPrefVelocity(simulator, e.uRVO.agentId, new Vector3(0, 0, 0));
                EntityHelper.SyncEntityStatus(e, aoi, (int)CharactorStatus.Idle);
                return;
            }

            v.x = dir.x * e.uProps.speed / len;
            v.z = dir.z * e.uProps.speed / len;
            v.y = 0;
            SetAgentPrefVelocity(simulator, e.uRVO.agentId, v);
        }

        public static void RvoLateUpdate(Simulator simulator, AOIMgr aoi, BaseEntity e, float dt)
        {

            RVO.Vector2 pos = simulator.getAgentPosition(e.uRVO.agentId);
            RVO.Vector2 vel = simulator.getAgentVelocity(e.uRVO.agentId);
            e.uTransform.pos = new Vector3(pos.x(), 0, pos.y());

            if (Math.Abs(vel.x()) > 0.0005f && Math.Abs(vel.y()) > 0.0005f)
            {
                e.uTransform.eulerAngles.y = (MathF.Atan2(vel.y(), vel.x()) * 180) / MathF.PI;
            }

        }

        public static int CreateAgent(Simulator simulator, Vector3 pos, float r)
        {
            int agentId = simulator.addAgent(new RVO.Vector2(pos.x, pos.z));
            simulator.setAgentRadius(agentId, r);
            return agentId;
        }

        public static void RVOAddObstacles(Simulator simulator, List<Vector3[]> obsSet)
        {
            for (int i = 0; i < obsSet.Count; i++)
            {
                List<RVO.Vector2> obsData = new List<RVO.Vector2>();
                for (int j = 0; j < obsSet[i].Length; j++)
                {
                    obsData.Add(new RVO.Vector2(obsSet[i][j].x, obsSet[i][j].z));
                }

                simulator.addObstacle(obsData);
            }

            simulator.processObstacles();

        }

        public static void SetAgentPrefVelocity(Simulator simulator, int agentId, Vector3 v)
        {
            simulator.setAgentPrefVelocity(agentId, new RVO.Vector2(v.x, v.z));
        }

        public static void SetAgentPosition(Simulator simulator, int agentId, Vector3 v)
        {
            simulator.setAgentPosition(agentId, new RVO.Vector2(v.x, v.z));
        }

        public static Vector3 GetAgentPosition(Simulator simulator, int agentId)
        {
            RVO.Vector2 pos = simulator.getAgentPosition(agentId);

            return new Vector3(pos.x(), 0, pos.y());
        }

        public static float GetAgentRaduis(Simulator simulator, int agentId)
        {
            return simulator.getAgentRadius(agentId);
        }

        public static void DestroyAgent(Simulator simulator, int agentId)
        {
            simulator.delAgent(agentId);
        }

        public static void ClearRVO(Simulator simulator)
        {
            simulator.Clear();
        }
    }
}
