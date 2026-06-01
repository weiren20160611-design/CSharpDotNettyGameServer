
using Framework.Core.Utils;
using Game.Core.Db;
using Game.Datas.DBEntities;
using Game.Datas.Excels;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

//任务状态说明：0 未开启  1 进行中  2结束完成  3处理了结束流程
namespace Game.Core.GM_Task
{
    public enum TaskStatus
    {
        Invalid = -1,
        UnStart = 0,
        Starting = 1,
        Completed = 2,
        EndComplete = 3
    }
    public class GM_Task
    {
        public Gametask taskInstanceDb;
        public string taskDesic = "";
        public object taskProgressData;

        public static GM_Task Create(long uid, int tid)
        {
            GM_Task gM_Task = new GM_Task();
            gM_Task.taskInstanceDb = new Gametask();
            //gM_Task.taskInstanceDb.id = IdGenerator.GetNextId();
            gM_Task.taskInstanceDb.endTime = -1;
            gM_Task.taskInstanceDb.startTime = UtilsHelper.Timestamp();
            gM_Task.taskInstanceDb.tid = tid;
            gM_Task.taskInstanceDb.uid = uid;
            gM_Task.taskInstanceDb.status = (int)TaskStatus.Starting;
            return gM_Task;
        }
    }

    public class GM_TaskLine
    {
        public int taskLineType;
        public List<GM_Task> taskSet = new List<GM_Task>();

    }

    public class GM_TaskMgr
    {
        public static GM_TaskMgr Instance = new GM_TaskMgr();
        private List<int> taskLineTypes = new List<int>();

        private Dictionary<int, MethodInfo> taskDataEncodeMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> taskDataDecodeMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> createAndStartTaskMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> taskIsCompletedMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> taskDoCompleteMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> updateTaskProgressMethods = new Dictionary<int, MethodInfo>();
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public void Init()
        {

            IEnumerable<Type> taskClasses = TypeScanner.ListTypesWithAttribute(typeof(TaskClass));
            foreach (Type taskClass in taskClasses)
            {
                int mainType = taskClass.GetCustomAttribute<TaskClass>().mainType;
                taskLineTypes.Add(mainType);
                MethodInfo[] methodInfos = taskClass.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    TaskDataEncode taskEncode = methodInfo.GetCustomAttribute<TaskDataEncode>();
                    if (taskEncode != null)
                    {
                        int key = mainType + taskEncode.subType;
                        if (taskDataEncodeMethods.ContainsKey(key))
                        {
                            logger.Error("重复注册任务编码方法：" + key);
                            continue;
                        }
                        taskDataEncodeMethods.Add(key, methodInfo);
                        continue;
                    }

                    TaskDataDecode taskDecode = methodInfo.GetCustomAttribute<TaskDataDecode>();
                    if (taskDecode != null)
                    {
                        int key = mainType + taskDecode.subType;
                        if (taskDataDecodeMethods.ContainsKey(key))
                        {
                            logger.Error("重复注册任务编码方法：" + key);
                            continue;
                        }
                        taskDataDecodeMethods.Add(key, methodInfo);
                        continue;
                    }

                    TaskIsStarted taskIsStart = methodInfo.GetCustomAttribute<TaskIsStarted>();
                    if (taskIsStart != null)
                    {
                        int key = mainType;
                        if (createAndStartTaskMethods.ContainsKey(key))
                        {
                            logger.Error("重复注册任务编码方法：" + key);
                            continue;
                        }
                        createAndStartTaskMethods.Add(key, methodInfo);
                        continue;
                    }

                    TaskIsCompleted isCompleted = methodInfo.GetCustomAttribute<TaskIsCompleted>();
                    if (isCompleted != null)
                    {
                        int key = mainType + isCompleted.subType;
                        if (taskIsCompletedMethods.ContainsKey(key))
                        {
                            logger.Error("重复注册任务编码方法：" + key);
                            continue;
                        }
                        taskIsCompletedMethods.Add(key, methodInfo);
                        continue;
                    }

                    TaskDoCompleteAction completeAction = methodInfo.GetCustomAttribute<TaskDoCompleteAction>();
                    if (completeAction != null)
                    {
                        int key = mainType + completeAction.subType;
                        if (taskDoCompleteMethods.ContainsKey(key))
                        {
                            logger.Error("重复注册任务编码方法：" + key);
                            continue;
                        }
                        taskDoCompleteMethods.Add(key, methodInfo);
                        continue;
                    }

                    UpdateTaskProgress updateAction = methodInfo.GetCustomAttribute<UpdateTaskProgress>();
                    if (updateAction != null)
                    {
                        int key = mainType + updateAction.subType;
                        if (updateTaskProgressMethods.ContainsKey(key))
                        {
                            logger.Error("重复注册任务编码方法：" + key);
                            continue;
                        }
                        updateTaskProgressMethods.Add(key, methodInfo);
                        continue;
                    }

                }
            }
        }


        public MethodInfo GetMethod(Dictionary<int, MethodInfo> funcDic, int taskTypeId)
        {
            MethodInfo method = null;
            int taskLineType = ((int)taskTypeId / 100000) * 100000;
            if (funcDic.ContainsKey(taskTypeId))
            {
                method = funcDic[taskTypeId];
            }
            else if (funcDic.ContainsKey(taskLineType - 1))
            {
                method = funcDic[taskLineType - 1];
            }
            return method;
        }


        public void LoadTasksFromDb(GM_PlayerEntity gM_PlayerEntity)
        {
            Gametask[] gametasks = DBService.Instance.GetGameInstance().Queryable<Gametask>().Where(it => it.uid == gM_PlayerEntity.uPlayer.playerInfo.id).ToArray();
            for (int i = 0; i < taskLineTypes.Count; i++)
            {
                CheckAndStartTask(gM_PlayerEntity, taskLineTypes[i], gametasks);
            }
        }


        private void StartTask(GM_PlayerEntity gM_PlayerEntity, int taskLineType, Gametask gametask)
        {
            GM_TaskLine taskLine = new GM_TaskLine();
            taskLine.taskLineType = taskLineType;
            taskLine.taskSet = new List<GM_Task>();
            GM_Task task = new GM_Task();
            task.taskInstanceDb = gametask;
            MethodInfo method = GetMethod(taskDataDecodeMethods, (int)gametask.tid);

            task.taskDesic = "";
            if (method != null)
            {
                method.Invoke(null, new object[] { task });
            }
            taskLine.taskSet.Add(task);
            gM_PlayerEntity.uTask.taskSet.TryAdd(taskLineType, taskLine);
        }

        /// <summary>
        /// 检查任务是否开始
        /// </summary>
        /// <param name="gM_PlayerEntity"></param>
        /// <param name="taskLineType"></param>
        /// <param name="gametasks"></param>
        public void CheckAndStartTask(GM_PlayerEntity gM_PlayerEntity, int taskLineType, Gametask[] gametasks)
        {
            int nextTaskId = taskLineType;
            for (int i = 0; i < gametasks.Length; i++)
            {
                if (gametasks[i].tid < taskLineType || gametasks[i].tid > taskLineType + 99999)
                {
                    continue;
                }


                if (gametasks[i].status > (int)TaskStatus.Starting)
                {
                    nextTaskId = (int)gametasks[i].tid;
                    continue;
                }
                else if (gametasks[i].status == (int)TaskStatus.Starting)
                {
                    StartTask(gM_PlayerEntity, taskLineType, gametasks[i]);
                    return;
                }

                nextTaskId = (int)gametasks[i].tid;
            }

            nextTaskId++;

            LoadNewTask(gM_PlayerEntity, taskLineType, nextTaskId);

        }

        public GM_Task GetCurrentTask(GM_PlayerEntity gM_PlayerEntity, int taskLineType)
        {
            if (gM_PlayerEntity.uTask.taskSet.TryGetValue(taskLineType, out GM_TaskLine taskLine))
            {
                for (int i = 0; i < taskLine.taskSet.Count; i++)
                {
                    if (taskLine.taskSet[i].taskInstanceDb.status == (int)TaskStatus.Starting)
                    {
                        return taskLine.taskSet[i];
                    }
                }
            }
            return null;
        }

        public void UpdateTaskProgress(GM_PlayerEntity gM_PlayerEntity, GM_Task gM_Task, string key, int num, ResTestGetGoods res)
        {
            MethodInfo method = GetMethod(updateTaskProgressMethods, (int)gM_Task.taskInstanceDb.tid);
            if (method != null)
            {
                method.Invoke(null, new object[] { gM_Task, key, num });
            }
            if (!CheckTaskIsComplete(gM_PlayerEntity, gM_Task))
            {
                UpdateOrInsertTaskToDb(gM_Task);
                res.status = (int)Respones.CollectSuccess;
            }
            else
            {
                res.status = (int)Respones.TaskDone;
            }
        }

        public void UpdateOrInsertTaskToDb(GM_Task gM_Task, bool isInsert = false)
        {
            MethodInfo method = GetMethod(taskDataEncodeMethods, (int)gM_Task.taskInstanceDb.tid);
            if (method != null)
            {
                byte[] data = (byte[])method.Invoke(null, new object[] { gM_Task.taskProgressData });
                gM_Task.taskInstanceDb.taskData = data;
                if (isInsert)
                {
                    InsertToDb(gM_Task.taskInstanceDb);
                }
                else
                {
                    UpdateToDb(gM_Task.taskInstanceDb);
                }
            }
        }

        private async void InsertToDb(Gametask gametask)
        {
            long newId = await DBService.Instance.GetGameInstance().Insertable(gametask).ExecuteReturnIdentityAsync();
            gametask.id = newId;
        }

        private void UpdateToDb(Gametask gametask)
        {
            DBService.Instance.GetGameInstance().Updateable(gametask).Where(it => it.tid == gametask.tid).ExecuteCommand();
        }


        public bool CheckTaskIsComplete(GM_PlayerEntity gM_PlayerEntity, GM_Task gM_Task)
        {
            MethodInfo method = GetMethod(taskIsCompletedMethods, (int)gM_Task.taskInstanceDb.tid);
            if (method != null)
            {
                bool isCompleted = (bool)method.Invoke(null, new object[] { gM_PlayerEntity, gM_Task });
                if (isCompleted)
                {
                    gM_Task.taskInstanceDb.status = (int)TaskStatus.Completed;
                    DoTaskCompletedAction(gM_PlayerEntity, gM_Task);
                    return true;
                }
            }
            else
            {
                logger.Error("taskDataCheckIfCompleteMethods not found key:" + gM_Task.taskInstanceDb.tid);
            }
            return false;
        }

        private void DoTaskCompletedAction(GM_PlayerEntity gM_PlayerEntity, GM_Task gM_Task)
        {
            MethodInfo method = GetMethod(taskDoCompleteMethods, (int)gM_Task.taskInstanceDb.tid);
            if (method != null)
            {
                method.Invoke(null, new object[] { gM_PlayerEntity, gM_Task });
            }
            else
            {
                logger.Error("taskDataCheckIfCompleteMethods not found key:" + gM_Task.taskInstanceDb.tid);
            }
            gM_Task.taskInstanceDb.status = (int)TaskStatus.EndComplete;
            UpdateToDb(gM_Task.taskInstanceDb);

            //默认加载下一个任务
            int defaultKey = ((int)gM_Task.taskInstanceDb.tid / 100000) * 100000;
            LoadNewTask(gM_PlayerEntity, defaultKey, (int)gM_Task.taskInstanceDb.tid + 1);
        }

        public void LoadNewTask(GM_PlayerEntity gM_PlayerEntity, int taskLineType, int nextTaskId)
        {
            GM_TaskLine taskLine = null;

            gM_PlayerEntity.uTask.taskSet.TryGetValue(taskLineType, out taskLine);

            MethodInfo method = null;
            if (createAndStartTaskMethods.ContainsKey(taskLineType))
            {
                method = createAndStartTaskMethods[taskLineType];
                if (method == null)
                {
                    return;
                }
                GM_Task gM_Task = (GM_Task)method.Invoke(null, new object[] { gM_PlayerEntity, taskLineType, nextTaskId });
                if (gM_Task != null)
                {
                    if (taskLine == null)
                    {
                        taskLine = new GM_TaskLine();
                        taskLine.taskLineType = taskLineType;
                        gM_PlayerEntity.uTask.taskSet.TryAdd(taskLineType, taskLine);
                    }

                    taskLine.taskSet.Add(gM_Task);
                    UpdateOrInsertTaskToDb(gM_Task, true);
                }
            }
        }

        public List<GM_Task> PullingTaksData(GM_PlayerEntity playerEntity, int taskTypeId)
        {
            List<GM_Task> tasks = new List<GM_Task>();
            for (int i = 0; i < taskLineTypes.Count; i++)
            {
                GM_TaskLine gM_TaskLine = null;
                playerEntity.uTask.taskSet.TryGetValue(taskLineTypes[i], out gM_TaskLine);
                if (gM_TaskLine == null)
                {
                    continue;
                }
                for (int j = 0; j < gM_TaskLine.taskSet.Count; j++)
                {
                    if (gM_TaskLine.taskSet[j].taskInstanceDb.status != (int)TaskStatus.Starting &&
                        gM_TaskLine.taskSet[j].taskInstanceDb.status != (int)TaskStatus.UnStart)
                    {
                        continue;
                    }
                    tasks.Add(gM_TaskLine.taskSet[j]);
                }
            }
            return tasks;
        }


    }
}
