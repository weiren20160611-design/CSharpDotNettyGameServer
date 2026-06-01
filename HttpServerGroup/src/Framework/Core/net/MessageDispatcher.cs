using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Framework.Core.Serializer;
using Framework.Core.task;
using Framework.Core.Utils;
using Game;
using Game.Datas.Messages;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Reflection;

namespace Framework.Core.Net
{
    class MessageDispatcher
    {
        public static MessageDispatcher Instance = new MessageDispatcher();

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /** [module_cmd, CmdExecutor] */
        private static Dictionary<string, CmdExecutor> MODULE_CMD_HANDLERS = new();

        public short[] GetMessageMeta(MethodInfo method)
        {
            foreach (ParameterInfo parameter in method.GetParameters())
            {
                if (parameter.ParameterType.IsAssignableTo(typeof(Message)))
                {
                    MessageMeta msgMeta = parameter.ParameterType.GetCustomAttribute<MessageMeta>();
                    if (msgMeta != null)
                    {
                        short[] meta = { msgMeta.module, msgMeta.cmd };
                        return meta;
                    }
                }
            }

            return null;
        }

        private string BuildKey(short module, short cmd)
        {
            return module + "_" + cmd;
        }

        public void Init()
        {  // 初始化
            IEnumerable<Type> controllers = TypeScanner.ListTypesWithAttribute(typeof(Controller));
            foreach (Type controller in controllers)
            {
                try
                {
                    object handler = Activator.CreateInstance(controller);
                    MethodInfo[] methods = controller.GetMethods();

                    foreach (MethodInfo method in methods)
                    {
                        RequestMapping mapperAttribute = method.GetCustomAttribute<RequestMapping>();
                        if (mapperAttribute == null)
                        {
                            continue;
                        }

                        short[] meta = this.GetMessageMeta(method);
                        short module = meta[0];
                        short cmd = meta[1];

                        string key = BuildKey(module, cmd);
                        MODULE_CMD_HANDLERS.TryGetValue(key, out CmdExecutor cmdExecutor);

                        if (cmdExecutor != null)
                        {
                            logger.Warn($"module[{module}] cmd[{cmd}] 重复注册处理器");
                            return;
                        }

                        cmdExecutor = CmdExecutor.Create(method, method.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray(), handler);

                        MODULE_CMD_HANDLERS.Add(key, cmdExecutor);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
        }

        // 有新的客户端进来
        public void OnClientEnter(IdSession s, bool isWebSocket = false)
        {
            string socketType = isWebSocket ? "WebSocket" : "TcpSocket";
            this.logger.Debug($"On Client Enter [{socketType}]:{s.GetRemoteAddress()}");
        }

        // 有客户端离开;
        public void OnClientExit(IdSession s, bool isWebSocket = false)
        {
            string socketType = isWebSocket ? "WebSocket" : "TcpSocket";
            this.logger.Debug($"On Client Exit [{socketType}]:{s.GetRemoteAddress()}");
            if (s.logicServerId != -1)
            {
                short module = (short)Game.Module.SCENE;
                short cmd = (short)Cmd.eQuitLogicServerReq;
                ReqQuitLogicServer req = new ReqQuitLogicServer();
                req.quitReason = (int)QuitReason.DisconnectQuit;
                LogicWorkerPool.Instance.PushMsgToLogicServer(s, module, cmd, req);
            }
        }

        private object[] ConvertToMethodParams(IdSession session, Type[] methodParams, Message message)
        {
            object[] result = new object[methodParams == null ? 0 : methodParams.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Type param = methodParams[i];
                if (param.IsAssignableTo(typeof(IdSession)))
                {
                    result[i] = session;
                }
                else if (param.IsAssignableTo(typeof(long)))
                {
                    result[i] = session.accountID;
                }
                else if (param.IsAssignableTo(typeof(Message)))
                {
                    result[i] = message;
                }
            }

            return result;
        }

        // 收到某个客户端的消息;
        public void OnClientMsg(IdSession s, byte[] data, int offset, int count)
        {
            // this.logger.Debug($"On Client Recv Cmd: {s.GetRemoteAddress()}");

            short module = UtilsHelper.ReadShortLE(data, offset + 0); // LE的方式
            short cmd = UtilsHelper.ReadShortLE(data, offset + 2);

            // 网关,可能要用到utag, 暂时保留;
            uint utag = UtilsHelper.ReadUintLE(data, offset + 4);


            Message msg = SerializerHelper.PbDecode(module, cmd, data, offset + 8, count - 8);
            string key = BuildKey(module, cmd);
            MODULE_CMD_HANDLERS.TryGetValue(key, out CmdExecutor cmdExecutor);
            if (cmdExecutor != null)
            {
                // 处理函数反射时候需要用到的参数;
                object[] @params = ConvertToMethodParams(s, cmdExecutor.@params, msg);
                long taskId = s.sessionId;//使用会话id作为任务ID
                TaskWorkerPool.Instance.AddTask(MessageTask.Create(taskId, cmdExecutor.handler, cmdExecutor.method, @params, s));
                return;
            }
            else
            {
                LogicWorkerPool.Instance.PushMsgToLogicServer(s, module, cmd, msg);
            }



        }
    }

}


