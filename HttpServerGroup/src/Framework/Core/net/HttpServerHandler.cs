namespace Framework.Core.Net {
    
    using DotNetty.Codecs.Http;
    using DotNetty.Common.Utilities;
    using DotNetty.Transport.Channels;
    using System;

 

    sealed class HttpServerHandler : ChannelHandlerAdapter {


        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public override void ChannelRead(IChannelHandlerContext ctx, object message) {
            if (message is IHttpRequest request)
            {
                try
                {
                    this.Process(ctx, request);
                }
                finally
                {
                    ReferenceCountUtil.Release(message);
                }
            }
            else
            {
                ctx.FireChannelRead(message);
            }
        }

        void Process(IChannelHandlerContext ctx, IHttpRequest request) {            
            HttpRouteDispatcher.Instance.OnHttpRequrestProcess(ctx, request);
        }

        // 检查请求来源是否是浏览器
        private bool IsBrowser(string userAgent)
        {
            // 简单检查浏览器常见标识
            return userAgent.Contains("Mozilla") || userAgent.Contains("Chrome") || userAgent.Contains("Safari") || userAgent.Contains("Firefox");
        }


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception) => context.CloseAsync();

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
    }
}

