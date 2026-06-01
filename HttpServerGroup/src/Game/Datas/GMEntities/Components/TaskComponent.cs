using Game.Core.GM_Task;
using Game.Datas.DBEntities;
using System.Collections.Concurrent;

namespace Game.Datas.GMEntities
{
    public struct TaskComponent
    {
        public ConcurrentDictionary<long, GM_TaskLine> taskSet;        
        public static void Init(GM_PlayerEntity playerEntity)
        {
            playerEntity.uTask.taskSet = new ConcurrentDictionary<long, GM_TaskLine>();
            GM_TaskMgr.Instance.LoadTasksFromDb(playerEntity);
        }

        public static void Exit(GM_PlayerEntity playerEntity)
        {

        }
    }
}