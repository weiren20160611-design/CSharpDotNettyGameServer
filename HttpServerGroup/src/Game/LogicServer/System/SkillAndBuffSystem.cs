using Game.Datas.GMEntities;
using Game.LogicServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.LogicServer
{
    public class SkillAndBuffSystem
    {
        public static void Update(AOIMgr aoi, GM_PlayerEntity e, float dt)
        {
            e.uSkillAndBuff.skillTimeLine.OnUpdate(dt);
            e.uSkillAndBuff.buffTimeLine.OnUpdate(dt);
        }

        public static void Update(RoomMgr room, GM_PlayerEntity e, float dt) {
            e.uSkillAndBuff.skillTimeLine.OnUpdate(dt);
            e.uSkillAndBuff.buffTimeLine.OnUpdate(dt);
        }
    }
}
