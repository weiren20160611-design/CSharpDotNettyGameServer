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

namespace Game
{
    public class GameServer
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static GameServer Instance = new GameServer();

        NettySocketServer server = null;

        private void StartNetSocketServer()
        {
            this.server = new NettySocketServer();
            bool IsSsl = bool.Parse(ConfigurationManager.AppSettings["IsSsl"]);

            bool OnlyHttpServerZone = bool.Parse(ConfigurationManager.AppSettings["OnlyHttpServerZone"]);
            if (OnlyHttpServerZone)
            {
                int HttpServerZonePort = int.Parse(ConfigurationManager.AppSettings["HttpServerZonePort"]);
                this.server.StartHttpServer(HttpServerZonePort, IsSsl).Wait();
                return;
            }

            bool GatewayMode = bool.Parse(ConfigurationManager.AppSettings["GatewayMode"]);
            bool asGateway = bool.Parse(ConfigurationManager.AppSettings["AsGateway"]);
            bool asServer = bool.Parse(ConfigurationManager.AppSettings["AsServer"]);

            int tcpPort = int.Parse(ConfigurationManager.AppSettings["port"]);
            int wsPort = int.Parse(ConfigurationManager.AppSettings["wsPort"]);
            int httpPort = int.Parse(ConfigurationManager.AppSettings["httpPort"]);
            int GwConnectPort = int.Parse(ConfigurationManager.AppSettings["GwConnectPort"]);

            int GatewayTcpPort = int.Parse(ConfigurationManager.AppSettings["GatewayTcpPort"]);
            int GatewayWsPort = int.Parse(ConfigurationManager.AppSettings["GatewayWsPort"]);
            int GatewayHttpPort = int.Parse(ConfigurationManager.AppSettings["GatewayHttpPort"]);



            if (GatewayMode == false)
            {
                this.server.StartSterverAt(tcpPort, wsPort, IsSsl).Wait();
                this.server.StartHttpServer(httpPort, IsSsl).Wait();
            }
            else
            {
                if (asGateway)
                {
                    this.server.StartGatewaySterverAt(GatewayTcpPort, GatewayWsPort, IsSsl).Wait();
                    this.server.StartHttpServer(GatewayHttpPort, IsSsl).Wait();
                }
                if (asServer)
                {
                    this.server.StartGwToServerSocketAt(GwConnectPort).Wait();
                }
                if (asGateway)
                {
                    GatewayMgr.Instance.GatewayConnectToGameServer().Wait();
                }
            }
        }

        private void FrameworkInit()
        {


            // 加载我们的配置文件，如果默认是nlog.config,会被自动加载
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config");
            MessageFactory.Instance.InitMeesagePool();

            // 初始化带有Attribute属性消息分发器
            MessageDispatcher.Instance.Init();
            HttpRouteDispatcher.Instance.Init();

            SessionMgr.Instance.Init();
            bool asGateway = bool.Parse(ConfigurationManager.AppSettings["AsGateway"]);
            if (asGateway)
            {
                GatewayMgr.Instance.Init();
            }
            TaskWorkerPool.Instance.Start(5);
        }


        private void GameLogicInit()
        {

            // 初始化数据库服务
            DBService.Instance.Init();
            RedisService.Instance.Init();
            ConfigMgr.Instance.Init();
            HttpServerZoneModule.Instance.Init();
            bool OnlyHttpServerZone = bool.Parse(ConfigurationManager.AppSettings["OnlyHttpServerZone"]);
            if (OnlyHttpServerZone)
            {
                return;
            }

            // end
            BonuesMgr.Instance.Init();
            GM_SkillMgr.Instance.Init();
            GM_BuffMgr.Instance.Init();

            GM_EntityMgr.Instance.Init();
            GM_EmailMgr.Instance.Init();
            GM_TaskMgr.Instance.Init();
            GM_RankMgr.Instance.Init();
            GM_BackpackMgr.Instance.Init();
            GM_TradingMgr.Instance.Init();

            AuthModule.Instance.Init();
            PlayerModule.Instance.Init();
            LogicServerFactory.Instance.Init();
        }

        public void Start()
        {


            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            this.FrameworkInit();

            this.GameLogicInit();

            this.StartNetSocketServer();

            stopWatch.Stop();

            logger.Info($"游戏服务启动，耗时[{stopWatch.ElapsedMilliseconds}]毫秒");
        }

        public async Task Shutdown()
        {
            await this.server.Shutdown();

            TaskWorkerPool.Instance.Stop();
        }
    }
}



