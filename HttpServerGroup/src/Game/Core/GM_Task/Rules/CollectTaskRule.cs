using Framework.Core.Utils;
using Game.Core.Cache;
using Game.Datas.Excels;
using Game.Datas.GMEntities;
using Game.Utils;
using ProtoBuf;
using System.Collections.Generic;

namespace Game.Core.GM_Task
{
    [ProtoContract]
    class CollectProgressData
    {
        [ProtoMember(1, IsRequired = true)]
        public int damond;

        [ProtoMember(2, IsRequired = true)]
        public int book;

        public Dictionary<string, int> conds = new Dictionary<string, int>();
        public Dictionary<string, int> completeBonues = new Dictionary<string, int>();
    }
    [TaskClass(100000)]
    public class CollectTaskRule
    {

        [TaskDataEncode(-1)]
        public static byte[] DefaultEncodeTaskData(object taskData)
        {
            return GameUtils.EncodeObjectToBytes<CollectProgressData>(taskData);
        }

        [TaskDataDecode(-1)]
        public static void DefaultDecodeTaskData(GM_Task task)
        {
            task.taskProgressData = GameUtils.DecodeBytesToObject<CollectProgressData>(task.taskInstanceDb.taskData);
            CollectTask collectTask = ExcelUtils.GetConfigData<CollectTask>(task.taskInstanceDb.tid.ToString());
            if (collectTask != null)
            {
                task.taskDesic = collectTask.desicFormat;
            }
            CollectProgressData data = (CollectProgressData)task.taskProgressData;
            data.conds = GameUtils.ParseStringWithKeyAndValue(collectTask.DoneTaskCond);
            data.completeBonues = GameUtils.ParseStringWithKeyAndValue(collectTask.bonues);

        }


        [UpdateTaskProgress(-1)]
        public static void DefaultTaskUpdateProgress(GM_Task task, string key, object value)
        {
            CollectProgressData progressData = (CollectProgressData)task.taskProgressData;
            int oldValue = (int)GameUtils.GetFiled(progressData, key, 0);
            oldValue += (int)value;
            GameUtils.SetFiled(progressData, key, oldValue);
        }


        [TaskIsStarted()]
        public static GM_Task DefaultCreateAndStartTask(GM_PlayerEntity playerEntity, int taskLineType, int nextTaskId)
        {
            CollectTask collectTask = ExcelUtils.GetConfigData<CollectTask>(nextTaskId.ToString());
            if (collectTask == null)
            {
                return null;
            }
            GM_Task gM_Task = GM_Task.Create(playerEntity.uPlayer.playerInfo.id, nextTaskId);
            gM_Task.taskDesic = collectTask.desicFormat;
            CollectProgressData item = new CollectProgressData();
            item.conds = GameUtils.ParseStringWithKeyAndValue(collectTask.DoneTaskCond);
            item.completeBonues = GameUtils.ParseStringWithKeyAndValue(collectTask.bonues);
            gM_Task.taskProgressData = item;
            return gM_Task;


        }


        [TaskIsCompleted(-1)]
        public static bool DefaultCheckTaskIsCompleted(GM_PlayerEntity gM_PlayerEntities, GM_Task gM_Task)
        {
            CollectTask collectTask = ExcelUtils.GetConfigData<CollectTask>(gM_Task.taskInstanceDb.tid.ToString());
            if (collectTask == null)
            {
                return false;
            }
            CollectProgressData progressData = (CollectProgressData)gM_Task.taskProgressData;
            foreach (var key in progressData.conds.Keys)
            {
                int value = (int)GameUtils.GetFiled(progressData, key, 0);
                if (value < progressData.conds[key])
                {
                    return false;
                }
            }
            return true;
        }

        [TaskDoCompleteAction(-1)]
        public static void DefaultTaskDoCompleteAction(GM_PlayerEntity gM_PlayerEntities, GM_Task gM_Task)
        {
            CollectProgressData progressData = (CollectProgressData)gM_Task.taskProgressData;
            foreach (var key in progressData.completeBonues.Keys)
            {
                int value = (int)GameUtils.GetProperty(gM_PlayerEntities.uPlayer.playerInfo, key, 0);
                value += progressData.completeBonues[key];
                GameUtils.SetProperty(gM_PlayerEntities.uPlayer.playerInfo, key, value);
            }
            PlayerIDCache.Instance.UpdateDataToDB(gM_PlayerEntities.uPlayer.playerInfo);
        }
    }
}
