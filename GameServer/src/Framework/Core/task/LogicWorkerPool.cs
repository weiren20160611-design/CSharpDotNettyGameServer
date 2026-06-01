using Enyim.Caching.Configuration;
using Framework.Core.Net;
using Framework.Core.Utils;
using Game;
using Game.Core.EntityMgr;
using Game.Datas.Excels;
using Game.Datas.GMEntities;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Core.task
{

    public class LogicWorker : IDisposable
    {
        private readonly ConcurrentQueue<AbstractDistributeTask> _queue = new();
        private readonly AutoResetEvent _signal = new(false);
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _workerTask;
        private readonly Action<AbstractDistributeTask> _executor;
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; }

        public LogicWorker(string name, Action<AbstractDistributeTask> executor)
        {
            Name = name;
            _executor = executor;
            _workerTask = Task.Factory.StartNew(
                Run,
                _cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void Run()
        {
            try
            {
                while (!_cts.IsCancellationRequested)
                {
                    _signal.WaitOne(1000);


                    while (_queue.TryDequeue(out var task))
                    {
                        try
                        {
                            _executor(task);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"Worker {Name} 执行任务异常");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"Worker {Name} 线程异常退出");
            }
        }

        public void AddTask(AbstractDistributeTask task)
        {
            if (_cts.IsCancellationRequested)
                throw new InvalidOperationException($"Worker {Name} 已停止");

            _queue.Enqueue(task);
            _signal.Set();
        }

        public void Stop(int timeoutMs = 5000)
        {
            _cts.Cancel();
            _signal.Set();

            if (!_workerTask.Wait(timeoutMs))
                _logger.Warn($"Worker {Name} 未能在 {timeoutMs}ms 内退出");
        }

        public void Dispose()
        {
            Stop();
            _signal.Dispose();
            _cts.Dispose();
        }
    }

    public class LogicWorkerPool : IDisposable
    {
        public static LogicWorkerPool Instance = new LogicWorkerPool();

        private readonly List<LogicWorker> _workers = new();
        private readonly Dictionary<int, LogicTask> _logicTaskDic = new();
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public bool Running { get; private set; }

        public void AddLogicWoker(int index)
        {
            this._logger.Info($"LogicWorkerPool:开启LogicWorker-{index}线程");
            if (Running) return;

            Running = true;
            _workers.Clear();

            _workers.Add(new LogicWorker($"LogicWorker-{index}", ExecTask));
        }

        private void ExecTask(AbstractDistributeTask task)
        {
            try
            {
                task.DoAction();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Message handler exception");
            }
        }

        public void AddTask(AbstractDistributeTask task)
        {
            if (!Running || _workers.Count == 0)
                return;

            int index = (int)(task.taskId % _workers.Count);
            _workers[index].AddTask(task);
        }

        public void Stop()
        {
            if (!Running) return;

            Running = false;

            foreach (var worker in _workers)
                worker.Dispose();

            _workers.Clear();
        }

        public void Dispose()
        {
            Stop();
        }


        public void LogicServerBindLogicTaskThread(Type type, LogicServerInstance config, out BaseLogicServer handler)
        {
            handler = (BaseLogicServer)Activator.CreateInstance(type);
            if (handler == null)
            {
                _logger.Warn($"failed to create instance:{config.logicServerTid}");
                return;
            }

            handler.logicServerId = config.ID;
            handler.logicServerTid = config.logicServerTid;

            MethodInfo method = type.GetMethod("MainLoop");
            LogicServerMeta meta = type.GetCustomAttribute<LogicServerMeta>();

            long taskId = config.ID;
            var task = LogicTask.Create(taskId, meta.logicServerTid, handler, method);
            handler.Init(task.eventQueue, config);

            if (!_logicTaskDic.ContainsKey(handler.logicServerId))
            {
                _logicTaskDic.Add(handler.logicServerId, task);
            }

            AddTask(task);
        }

        public void PushMsgToLogicServer(IdSession s, short module, short cmd, object msg)
        {
            if (s.accountId <= 0 || s.playerId <= 0 || s.logicServerId == -1)
                return;

            if (!_logicTaskDic.TryGetValue(s.logicServerId, out var task))
            {
                _logger.Error($"[error]:{s.logicServerId} not exist");
                return;
            }

            task.PushEvent(s, s.playerId, module, cmd, msg);
        }

        public void GatewayPushMsgToLogicServer(long playerId, short logicServerId, short module, short cmd, object msg)
        {
            if (playerId <= 0 || logicServerId == -1)
                return;

            if (!_logicTaskDic.TryGetValue(logicServerId, out var task))
            {
                _logger.Error($"[error]:{logicServerId} not exist");
                return;
            }

            task.PushEvent(SessionMgr.Instance.gateWaySession, playerId, module, cmd, msg);
        }
    }

}

