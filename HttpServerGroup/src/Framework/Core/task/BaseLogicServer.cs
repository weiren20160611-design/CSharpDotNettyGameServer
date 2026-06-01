using Framework.Core.Net;
using Framework.Core.Serializer;
using Framework.Core.task;
using Framework.Core.Utils;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Core.task
{
    public abstract class BaseLogicServer
    {
        protected NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Queue<LogicEvent> eventQueue = null;// 事件队列
        MethodInfo OnUpdate = null;

        protected int fps = 20;
        protected long timePerFrame = 0;

        private long lastFrameMillis;

        public int logicServerId;// 逻辑服务器实例ID 从1开始
        public int logicServerTid; //逻辑服玩法类型ID
        public int onelinePlayers = 0; // 当前在线人数
        protected object instanceConfig = null;// 逻辑服配置文件实例

        protected TimerMgr timerMgr = null;
        private Dictionary<int, MethodInfo> keyMethodDic;


        public TimerMgr TimerMgr
        {
            get { return this.timerMgr; }
        }

        public void SetTargetFPS(int fps)
        {
            this.fps = fps;
            this.timePerFrame = 1000 / (fps);
        }

        public virtual void Init(Queue<LogicEvent> eventQueue, object instanceConfig = null)
        {
            this.instanceConfig = instanceConfig;
            this.eventQueue = eventQueue;
            this.onelinePlayers = 0;
            this.timerMgr = new TimerMgr();
            this.timerMgr.Init();
            this.SetTargetFPS(20);
            Type t = this.GetType();
            this.OnUpdate = t.GetMethod("OnUpdate");

            MethodInfo OnStart = t.GetMethod("OnStart");
            if (OnStart != null)
            {
                OnStart.Invoke(this, null);
            }


            this.keyMethodDic = new Dictionary<int, MethodInfo>();
            MethodInfo[] methods = t.GetMethods();
            foreach (MethodInfo method in methods)
            {
                RequestMapping mapperAttribute = method.GetCustomAttribute<RequestMapping>();
                if (mapperAttribute == null)
                {
                    continue;
                }


                short[] meta = MessageDispatcher.Instance.GetMessageMeta(method);
                short cmd = meta[1];


                if (keyMethodDic.ContainsKey(cmd))
                {
                    throw new RuntimeBinderException($"[致命错误]:[{cmd}]重复注册");
                }

                keyMethodDic.Add(cmd, method);
            }


            this.lastFrameMillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

        }

        public bool IsAppRunning()
        {
            return (LogicWorkerPool.Instance.Running);
        }

        private void ProcessNetEvents()
        {

            lock (this.eventQueue)
            {
                while (this.eventQueue.Count > 0)
                {
                    LogicEvent e = this.eventQueue.Dequeue();

                    if (this.keyMethodDic.ContainsKey(e.cmd))
                    {
                        var ret = this.keyMethodDic[e.cmd].Invoke(this, new object[] { e.s, e.msg });
                        if (ret != null)
                        {
                            MessagePusher.PushMessage(e.s, ret as Message);
                        }
                    }
                }
            }
        }


        public async Task MainLoop()
        {

            while (this.IsAppRunning())
            {
                // Wait For Target FPS
                long nowMillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                if (nowMillis - this.lastFrameMillis < this.timePerFrame)
                {
                    int sleepTime = (int)(this.timePerFrame - (nowMillis - this.lastFrameMillis));
                    await Task.Delay(sleepTime);
                    nowMillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                }

                long dt = (nowMillis - this.lastFrameMillis);
                this.lastFrameMillis = nowMillis;

                // 处理所有的事件
                this.ProcessNetEvents();
                // end

                this.timerMgr.Update(dt);

                // 迭代游戏逻辑
                if (this.OnUpdate != null)
                {
                    float duration = ((float)(dt)) / 1000.0f;
                    this.OnUpdate.Invoke(this, new object[] { duration });
                }
            }
        }

        

        #region 逻辑服通用接口
        public abstract int EnterLogicServer(long playerId, int zoneId, int logicServerId);
        public abstract int ReConnectGameInLogicServer(long playerId);
        public abstract int QuitLogicServer(long playerId, int reason);
        public abstract bool HasZone(int zoneId);
        #endregion
    }
}
