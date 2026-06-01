using Game.Datas.GMEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.GM_Task
{
    [TaskClass(200000)]
    class DemoATaskRule
    {
        [TaskDataEncode(-1)]
        public static byte[] DefaultEncodeTaskData(object taskData)
        {
            return null;
        }


        [TaskDataDecode(-1)]
        public static void DefaultDecodeTaskData(GM_Task task)
        {

        }

        [UpdateTaskProgress(-1)]
        public static void DefaultTaskUpdateProgress(GM_Task task, string key, object value)
        {

        }

        [TaskIsStarted()]
        public static object DefaultCheckTaskIsStart(GM_PlayerEntity playerEntity, int taskLineType, int nextTaskId)
        {
            return null;
        }

        [TaskIsCompleted(-1)]
        public static bool DefaultCheckIsCompleted(GM_PlayerEntity entity, GM_Task task)
        {
            return false;
        }

        [TaskDoCompleteAction(-1)]
        public static void DefaultTaskDoCompleteAction(GM_PlayerEntity gM_PlayerEntities, GM_Task gM_Task)
        {

        }

    }
}
