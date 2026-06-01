using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Framework.Core.Serializer;
using Framework.Core.Utils;
using Game;
using Game.Datas.Excels;
using Game.Datas.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Framework.Core.Net
{
    /// <summary>
    /// 网关作为中间件，处理游戏服和客户端的通信
    /// </summary>
    public class GameServerClientSocketHandler : ChannelHandlerAdapter
    {

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private const int HEAD_SIZE = 14; // [2, 2, 8, 2]

        public GameServerClientSocketHandler() : base()
        {
        }

        // 网关连接上了GameServer
        public override void ChannelActive(IChannelHandlerContext context)
        {
            // 连接进来的
            IChannel channel = context.Channel;
        }


        public void GatewayRecvMsgFromGameServer(byte[] data, int offset, int count)
        {
            // this.logger.Debug($"On Client Recv Cmd: {s.GetRemoteAddress()}");

            short module = UtilsHelper.ReadShortLE(data, offset + 0); // LE的方式
            short cmd = UtilsHelper.ReadShortLE(data, offset + 2);

            // 网关,可能要用到utag, 暂时保留;
            long utag = UtilsHelper.ReadLongLE(data, offset + 4);

            IdSession clientSession = GatewayMgr.Instance.GetClientIdSession(utag);
            if (clientSession == null)
            {
                this.logger.Error($"Find ClientSession Error");
                return;
            }


            if (cmd == (short)Cmd.eGuestLoginRes)
            {
                Message msg = SerializerHelper.PbDecode(module, cmd, data, offset + HEAD_SIZE, count - HEAD_SIZE);
                ResGuestLogin res = msg as ResGuestLogin;
                clientSession.accountId = res.accountId;
                res.accountId = 0;
                MessagePusher.PushMessage(clientSession, msg, 0, 0);
                return;
            }
            else if (cmd == (short)Cmd.eUserLoginRes)
            {
                Message msg = SerializerHelper.PbDecode(module, cmd, data, offset + HEAD_SIZE, count - HEAD_SIZE);
                ResUserLogin res = msg as ResUserLogin;
                clientSession.accountId = res.accountId;
                res.accountId = 0;
                MessagePusher.PushMessage(clientSession, msg, 0, 0);
                return;
            }
            else if (cmd == (short)Cmd.eSelectPlayerRes)
            {
                Message msg = SerializerHelper.PbDecode(module, cmd, data, offset + HEAD_SIZE, count - HEAD_SIZE);
                ResSelectPlayer res = msg as ResSelectPlayer;
                clientSession.playerId = res.playerId;
                res.playerId = 0;
                MessagePusher.PushMessage(clientSession, msg, 0, 0);
                return;
            }
            else if (cmd == (short)Cmd.ePullingPlayerDataRes)
            {
                Message msg = SerializerHelper.PbDecode(module, cmd, data, offset + HEAD_SIZE, count - HEAD_SIZE);
                ResPullingPlayerData res = msg as ResPullingPlayerData;
                clientSession.playerId = res.playerId;
                res.playerId = 0;
                MessagePusher.PushMessage(clientSession, msg, 0, 0);
                return;
            }
            else if (cmd == (short)Cmd.eEnterLogicServercRes)
            {
                short logicServerId = UtilsHelper.ReadShortLE(data, offset + 12);
                clientSession.logicServerId = logicServerId;
            }
            else if (cmd == (short)Cmd.eQuitLogicServerRes)
            {
                Message msg = SerializerHelper.PbDecode(module, cmd, data, offset + HEAD_SIZE, count - HEAD_SIZE);
                ResQuitLogicServer res = msg as ResQuitLogicServer;
                if (res.status == (int)Respones.OK)
                {
                    clientSession.logicServerId = -1;
                }
            }


            UtilsHelper.WriteULongLE(data, offset + 4, 0);
            UtilsHelper.WriteShortLE(data, offset + 12, 0);

            clientSession.Send(data, offset, count);
        }

        // 网关收到GameServer 返回给我们的消息;
        public override void ChannelRead(IChannelHandlerContext context, object message)
        { // 有数据读的时候 
            var msg = message as IByteBuffer;
            IChannel channel = context.Channel;
            IdSession s = SessionMgr.Instance.GetSessionBy(channel);
            this.logger.Debug($"GameServerClientSocketHandler Recv Cmd: {s.GetRemoteAddress()}");
            // 网关收到来自游戏服务器的消息;
            this.GatewayRecvMsgFromGameServer(msg.Array, msg.ArrayOffset, msg.ReadableBytes);
            // end
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush(); // 数据读取完的一个处理

        // 网关与游戏服务器断开;
        public override void ChannelInactive(IChannelHandlerContext context)
        {
            IChannel channel = context.Channel;
            IdSession s = SessionMgr.Instance.GetSessionBy(channel);

            channel.DisconnectAsync();

            s.client = null;
            GatewayMgr.Instance.taskEvent.Set();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception e)
        { // 有异常的时候
            IChannel channel = context.Channel;
            if (channel.Active || channel.Open)
            {
                context.CloseAsync();
            }

            if (!(e is IOException))
            {
                this.logger.Debug("remote:" + channel.RemoteAddress, e.Message);
            }
        }
    }

    public class GatewayMgr
    {
        public static GatewayMgr Instance = new GatewayMgr();
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private long autoId = 0;


        private Dictionary<int, IdSession> machineSet = null; // 机器码===>GameServer Socket
        private AttributeKey<object> MACHINE_ID = AttributeKey<object>.ValueOf("MACHINE_ID");


        IEventLoopGroup gatewayGroup = null;
        Bootstrap bootStrap = null;
        private List<GatewayConfig> configs = null;



        public AutoResetEvent taskEvent = new AutoResetEvent(true);
        private bool isRunning = false;

        private Dictionary<long, IdSession> sessionIdMap = null;
        private Dictionary<long, IdSession> accountIdMap = null;
        private Dictionary<long, IdSession> playerIdMap = null;

        public void Init()
        {
            this.autoId = 0;


            this.machineSet = new Dictionary<int, IdSession>();
            this.sessionIdMap = new Dictionary<long, IdSession>();
            this.accountIdMap = new Dictionary<long, IdSession>();
            this.playerIdMap = new Dictionary<long, IdSession>();
            this.configs = ExcelUtils.GetConfigDatas<GatewayConfig>(null);
            // 加载机器配置表, 连接到其他的GameServer
            for (int i = 0; i < this.configs.Count; i++)
            {
                GatewayConfig config = this.configs[i];
                if (config.Enabled == 0)
                {
                    continue;
                }
                // 每个机器，创建一个空的IdSession对象;
                IdSession client = SessionMgr.Instance.CreateIdSession(null);
                // 机器码===》IdSession 映射;
                this.machineSet.Add(config.ID, client);
            }
            // end

            this.gatewayGroup = new MultithreadEventLoopGroup(4);
            this.bootStrap = new Bootstrap();
            this.bootStrap.Group(this.gatewayGroup)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast("framing-enc", new LengthFieldPrepender(ByteOrder.LittleEndian, 2, 0, true));
                    pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ByteOrder.LittleEndian, ushort.MaxValue, 0, 2, -2, 2, true));
                    pipeline.AddLast("IoEventHandler", new GameServerClientSocketHandler());
                }));


            this.taskEvent.Reset();
            this.isRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(GatewayReConnectThread), this);
        }

        private void GatewayReConnectThread(object stateInfo)
        {

            while (this.isRunning)
            {
                this.logger.Info("Waiting for Scan ReConnect ...");
                this.taskEvent.WaitOne();

                if (this.isRunning == false)
                { // 唤醒退出;
                    break;
                }
                this.logger.Info("Begin Scan ReConnect ...");
                this.ScaneReconnectToServer().Wait();
                this.logger.Info("End Scan ReConnect ...");
            }

            this.logger.Info("网关重连接扫描线程退出 ...");
        }

        public void Stop()
        {
            this.isRunning = false;
            this.taskEvent.Set();
        }

        private async Task ScaneReconnectToServer()
        {
            for (int i = 0; i < this.configs.Count; i++)
            {
                GatewayConfig config = this.configs[i];
                if (config.Enabled == 0 || this.machineSet[config.ID].client != null)
                {
                    continue;
                }

                await this.ConnectToServer(config);
            }
        }

        private async Task ConnectToServer(GatewayConfig config)
        {
            try
            {
                var remoteAddress = new IPEndPoint(IPAddress.Parse(config.IpAddr), config.ConnPort);
                this.logger.Info($"Gateway Connect To GameServer:{config.ID}-{config.IpAddr}:{config.ConnPort}");
                IChannel channel = await this.bootStrap.ConnectAsync(remoteAddress);
                SessionMgr.Instance.SetSessionChannel(this.machineSet[config.ID], channel);
            }
            catch (Exception e)
            {
                this.logger.Warn(e.ToString());
                this.taskEvent.Set();
            }

        }

        public async Task GatewayConnectToGameServer()
        {
            for (int i = 0; i < this.configs.Count; i++)
            {
                if (this.configs[i].Enabled == 0)
                {
                    continue;
                }
                await this.ConnectToServer(this.configs[i]);
            }
        }


        public IdSession GetClientIdSession(long utag)
        {
            if (this.playerIdMap.ContainsKey(utag))
            {
                return this.playerIdMap[utag];
            }

            if (this.accountIdMap.ContainsKey(utag))
            {
                return this.accountIdMap[utag];
            }

            if (this.sessionIdMap.ContainsKey(utag))
            {
                return this.sessionIdMap[utag];
            }

            return null;
        }


        private long GetClientUtagAndSaveClientSession(IdSession client, short module, short cmd)
        {
            CmdExecutor executor = MessageDispatcher.Instance.GetCmdExecutor(module, cmd);
            long utag = client.sessionId;
            if (executor == null)
            {
                utag = client.playerId;
                this.playerIdMap.TryAdd(utag, client);
                return utag;
            }
            else
            {
                for (int i = 0; i < executor.@params.Length; i++)
                {
                    if (executor.@params[i].ParameterType == typeof(long))
                    {
                        if (executor.@params[i].Name.Equals("accountId"))
                        {
                            utag = client.accountId;
                            this.accountIdMap.TryAdd(utag, client);
                            return utag;
                        }
                        else if (executor.@params[i].Name.Equals("playerId"))
                        {
                            utag = client.playerId;
                            this.playerIdMap.TryAdd(utag, client);
                            return utag;
                        }
                    }
                }
            }
            this.sessionIdMap.TryAdd(utag, client);
            return utag;
        }

        public void SednMsgToGameServer(IdSession client, byte[] data, int offset, int count)
        {
            short module = UtilsHelper.ReadShortLE(data, offset + 0); // LE的方式
            short cmd = UtilsHelper.ReadShortLE(data, offset + 2);

            int machineId = (module & 0xFF00) >> 8;
            module = (short)(module & 0x00FF);
            UtilsHelper.WriteShortLE(data, offset + 0, module);
            long utag = this.GetClientUtagAndSaveClientSession(client, module, cmd);
            UtilsHelper.WriteULongLE(data, offset + 4, (ulong)utag);
            short logicServerId = (short)client.logicServerId;
            UtilsHelper.WriteShortLE(data, offset + 12, logicServerId);


            if (cmd == (short)Cmd.eEnterLogicServercRes)
            {
                if (logicServerId != -1)
                {
                    ResEnterLogicServer res = new ResEnterLogicServer();
                    res.status = (int)Respones.AlreadyInLogicServer;
                    MessagePusher.PushMessage(client, res, 0, 0);
                    return;
                }
            }

            //通过机器码找到对应服务器的IdSession
            IdSession s = this.machineSet[machineId];
            if (s == null)
            {
                this.logger.Warn($"[机器码错误{machineId}]");
                return;
            }

            s.Send(data, offset, count);
        }

        public void RecvMsgFromGameServer()
        {

        }


        public void OnClientExitFromGateway(IdSession clientSession)
        {
            if (clientSession.logicServerId != -1)
            { // 说明玩家正在游戏
                ReqQuitLogicServer req = new ReqQuitLogicServer();
                req.quitReason = (int)QuitReason.DisconnectQuit;
                long utag = clientSession.playerId;
                short logicServerId = (short)clientSession.logicServerId;
                IdSession s = this.machineSet[clientSession.machineId];
                if (s != null)
                {
                    MessagePusher.PushMessage(s, req, s.playerId, logicServerId);
                }
            }

            if (this.sessionIdMap.ContainsKey(clientSession.sessionId))
            {
                this.sessionIdMap.Remove(clientSession.sessionId);
            }

            if (this.accountIdMap.ContainsKey(clientSession.accountId))
            {
                this.accountIdMap.Remove(clientSession.accountId);
            }

            if (this.playerIdMap.ContainsKey(clientSession.playerId))
            {
                this.playerIdMap.Remove(clientSession.playerId);
            }
        }

    }
}

