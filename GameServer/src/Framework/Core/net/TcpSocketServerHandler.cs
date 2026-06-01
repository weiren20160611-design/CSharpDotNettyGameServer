using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.IO;
using System.Text;

namespace Framework.Core.Net
{

    public class TcpSocketServerHandler : ChannelHandlerAdapter
    {

        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private bool isGateway = false;
        public TcpSocketServerHandler(bool isGateway = false) : base()
        {
            this.isGateway = isGateway;
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
            IdSession s = SessionMgr.Instance.CreateIdSession(channel);
            if (isGateway)
            {
                MessageDispatcher.Instance.OnClientEnterToGateway(s);
            }
            else
            {
                MessageDispatcher.Instance.OnClientEnter(s);
            }
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
            IdSession s = SessionMgr.Instance.GetSessionBy(channel);
            if (isGateway)
            {
                MessageDispatcher.Instance.OnClientMsgToGataway(s, msg.Array, msg.ArrayOffset, msg.ReadableBytes);
            }
            else
            {
                MessageDispatcher.Instance.OnClientMsg(s, msg.Array, msg.ArrayOffset, msg.ReadableBytes);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush(); // 数据读取完的一个处理

        public override void ChannelInactive(IChannelHandlerContext context)
        { // 关闭
            IChannel channel = context.Channel;
            IdSession s = SessionMgr.Instance.GetSessionBy(channel);
            if (isGateway)
            {
                MessageDispatcher.Instance.OnClientExitFromGateway(s);
            }
            else
            {
                MessageDispatcher.Instance.OnClientExit(s);
            }
            channel.DisconnectAsync();
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