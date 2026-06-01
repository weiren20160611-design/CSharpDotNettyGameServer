using Game.Core.Db;
using Game.Core.GM_Backpack;
using Game.Datas.DBEntities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game.Datas.GMEntities
{
    public enum CharactorStatus
    {
        Invalid = -1,
        Idle = 0,
        Run = 1,
        Dance = 2,
        Attack = 3,
        Die = 4,
    }
    public struct StatusComponent
    {
        public int status;

        public void Init(GM_PlayerEntity player)
        {
            player.uStatus.status = (int)CharactorStatus.Invalid;
        }
    }
}
