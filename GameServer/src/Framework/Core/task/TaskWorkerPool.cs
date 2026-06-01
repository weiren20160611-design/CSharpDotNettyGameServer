using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Framework.Core.Utils;
using Game.Datas.Excels;
using NLog;

namespace Framework.Core.task
{

    public class TaskWorker : IDisposable
    {
        private readonly ConcurrentQueue<AbstractDistributeTask> _queue = new();
        private readonly AutoResetEvent _signal = new(false);
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _workerTask;
        private readonly Action<AbstractDistributeTask> _executor;
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; }

        public TaskWorker(string name, Action<AbstractDistributeTask> executor)
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

                    while (_queue.TryDequeue(out var task2))
                    {
                        try
                        {
                            _executor(task2);
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




    /// <summary>
    /// 任务工作线程池
    /// </summary>
    public class TaskWorkerPool : IDisposable
    {
        public static TaskWorkerPool Instance = new TaskWorkerPool();

        private readonly List<TaskWorker> _workers = new();
        private readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public bool Running { get; private set; }

        public void Start(int threadCount)
        {
            this._logger.Info($"TaskWorkerPool:开始{threadCount}个线程");
            if (Running) return;

            if (threadCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(threadCount));

            Running = true;
            _workers.Clear();

            for (int i = 0; i < threadCount; i++)
            {
                _workers.Add(new TaskWorker($"TaskWorker-{i}", ExecTask));
            }
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
    }

}

