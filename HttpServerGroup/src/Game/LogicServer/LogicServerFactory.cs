using Framework.Core.task;
using Framework.Core.Utils;
using Game.Datas.Excels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game.LogicServer
{
    public class LogicServerFactory
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static LogicServerFactory Instance = new LogicServerFactory();
        //逻辑服ID->逻辑服对象
        private Dictionary<int, BaseLogicServer> logicServerIdMappingServer = new Dictionary<int, BaseLogicServer>();
        //逻辑服类型ID->逻辑服列表
        private Dictionary<int, List<BaseLogicServer>> logicServerTidMappingLogicServerList = new Dictionary<int, List<BaseLogicServer>>();
        //逻辑服类型ID->逻辑服类型
        private Dictionary<int, Type> logdicServerType = new Dictionary<int, Type>();
        public void Init()
        {
            IEnumerable<Type> servers = TypeScanner.ListTypesWithAttribute(typeof(LogicServerMeta));

            foreach (var server in servers)
            {
                LogicServerMeta meta = server.GetCustomAttribute<LogicServerMeta>();
                if (meta == null)
                {
                    continue;
                }
                if (logdicServerType.ContainsKey(meta.logicServerTid))
                {
                    logger.Error($"Duplicate registration of logical types:{meta.logicServerTid}");
                    continue;
                }
                logdicServerType.Add(meta.logicServerTid, server);
            }


            List<LogicServerInstance> configs = ExcelUtils.GetConfigDatas<LogicServerInstance>(null);
            for (int i = 0; i < configs.Count; i++)
            {
                LogicServerInstance config = configs[i];
                if (config.enabled == 1)
                {
                    LogicWorkerPool.Instance.AddLogicWoker(config.ID);
                    CreateLogicServerInstByConfig(config);
                }
            }
        }

        private void CreateLogicServerInstByConfig(LogicServerInstance config)
        {
            //检查是否存在玩法逻辑服
            if (!this.logdicServerType.ContainsKey(config.logicServerTid))
            {
                logger.Warn($"failed to start logical server:{config.logicServerTid}");
                return;
            }
            //创建一个逻辑服实例
            Type type = this.logdicServerType[config.logicServerTid];

            LogicWorkerPool.Instance.LogicServerBindLogicTaskThread(type, config, out BaseLogicServer logicServer);
            if (logicServer == null)
            {
                logger.Error($"failed to create logic server:{config.ID}");
                return;
            }
            logicServerIdMappingServer.Add(config.ID, logicServer);
            List<BaseLogicServer> serverList = null;
            if (!logicServerTidMappingLogicServerList.ContainsKey(config.logicServerTid))
            {
                serverList = new List<BaseLogicServer>();
                logicServerTidMappingLogicServerList.Add(config.logicServerTid, serverList);
            }
            else
            {
                serverList = logicServerTidMappingLogicServerList[config.logicServerTid];
            }
            serverList.Add(logicServer);

        }

        public bool IsExistLogicServer(int instanceId)
        {
            return logicServerIdMappingServer.ContainsKey(instanceId);
        }

        public BaseLogicServer GetLogicServer(int instanceId)
        {
            if (logicServerIdMappingServer.ContainsKey(instanceId))
            {
                return logicServerIdMappingServer[instanceId];
            }
            return null;
        }

        /// <summary>
        /// 查找人数最多的逻辑服务器的实例ID
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public int FindLogicServerInstance(int typeId, int zoneId)
        {
            if (!logicServerTidMappingLogicServerList.ContainsKey(typeId))
            {
                return -1;
            }
            int maxNum = -1;
            int index = -1;
            List<BaseLogicServer> serverList = logicServerTidMappingLogicServerList[typeId];
            serverList = serverList.OrderByDescending(p => p.onelinePlayers).ToList();
            for (int i = 0; i < serverList.Count; i++)
            {
                if (serverList[i].onelinePlayers > maxNum && serverList[i].HasZone(zoneId))
                {
                    maxNum = serverList[i].onelinePlayers;
                    index = i;
                }
            }

            if (index < 0)
            {
                return -1;
            }

            return serverList[index].logicServerId;
        }
    }
}
