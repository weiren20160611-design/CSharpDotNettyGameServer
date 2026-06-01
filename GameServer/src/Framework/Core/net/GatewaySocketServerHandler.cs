using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace Framework.Core.Net
{
    /// <summary>
    /// 网关到游戏服务器的SocketHandler(客户端先连接到网关服务器)
    /// </summary>
    public class GwToGameServerSocketHandler : ChannelHandlerAdapter
    {

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private bool AsAuthServer = false;
        public GwToGameServerSocketHandler() : base()
        {
            AsAuthServer = bool.Parse(ConfigurationManager.AppSettings["AsAuthServer"]);
        }
        /// <summary>
        /// 连接进入
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelActive(IChannelHandlerContext context)
        {
            // 连接进来的channel
            IChannel channel = context.Channel;
            // 创建一个会话对象(此对象代表着客户端的唯一标识)
            if (this.AsAuthServer)
            {
                IdSession s = SessionMgr.Instance.CreateIdSession(channel);
                MessageDispatcher.Instance.OnClientEnter(s);
            }
            else
            {
                SessionMgr.Instance.GateWayConnectToServer(channel);
            }
            //IdSession s = SessionMgr.Instance.gateWaySession; // 网关的Session
            this.logger.Debug($"Gateway Enter To GameServer: {channel.RemoteAddress.ToString()}");

        }
        /// <summary>
        /// 连接后读取数据
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            // 有数据读的时候 
            var msg = message as IByteBuffer;
            IChannel channel = context.Channel;
            // 获取当前channel的会话对象
            IdSession s = null;
            if (this.AsAuthServer)
            {
                s = SessionMgr.Instance.GetSessionBy(channel);
            }
            else
            {
                s = SessionMgr.Instance.gateWaySession;
            }
            MessageDispatcher.Instance.GatewaySendMsgToGameServer(s, msg.Array, msg.ArrayOffset, msg.ReadableBytes);

        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush(); // 数据读取完的一个处理

        public override void ChannelInactive(IChannelHandlerContext context)
        { // 关闭
            IChannel channel = context.Channel;
            if (this.AsAuthServer)
            {
                IdSession s = SessionMgr.Instance.GetSessionBy(channel);
                MessageDispatcher.Instance.OnClientExit(s);
            }
            else
            {
                SessionMgr.Instance.GatewayDisConnectFromServer();
            }
            channel.DisconnectAsync();
            this.logger.Debug($"Gateway Exit From GameServer: {channel.RemoteAddress.ToString()}");
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

}