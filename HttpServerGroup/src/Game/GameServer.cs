using Framework.Core.Net;
using Framework.Core.Serializer;
using Framework.Core.task;
using Game.Core.Config;
using Game.Core.Db;
using Game.Core.EmailMessage;
using Game.Core.EntityMgr;
using Game.Core.Entry.Modules;
using Game.Core.GameBonues;
using Game.Core.GM_Backpack;
using Game.Core.GM_Rank;
using Game.Core.GM_Task;
using Game.Core.GM_Trading;
using Game.LogicServer;
using NLog;
using NLog.Config;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Game {
    public class GameServer
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static GameServer Instance = new GameServer();

        NettySocketServer server = null;

        private void FrameworkInit() {

            
            // 加载我们的配置文件，如果默认是nlog.config,会被自动加载
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");
            MessageFactory.Instance.InitMeesagePool();

            // 初始化带有Attribute属性消息分发器
            MessageDispatcher.Instance.Init();
            HttpRouteDispatcher.Instance.Init();

            SessionMgr.Instance.Init(); 
            TaskWorkerPool.Instance.Start(5); 

            this.server = new NettySocketServer();
            bool IsSsl = bool.Parse(ConfigurationManager.AppSettings["IsSsl"]);
            int httpPort = int.Parse(ConfigurationManager.AppSettings["httpPort"]);
            this.server.StartHttpServer(httpPort, IsSsl).Wait();            
        }


        private void GameLogicInit() {

            HttpServerZoneModule.Instance.Init();
            // 初始化数据库服务
            DBService.Instance.Init();
            RedisService.Instance.Init();
            // end
            
        }

        public void Start() {


            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();


            this.FrameworkInit();

            this.GameLogicInit();

            stopWatch.Stop();

            logger.Info($"游戏服务启动，耗时[{stopWatch.ElapsedMilliseconds}]毫秒");
        }

        public async Task Shutdown() {
            await this.server.Shutdown();

            TaskWorkerPool.Instance.Stop();
        }
    }
}



