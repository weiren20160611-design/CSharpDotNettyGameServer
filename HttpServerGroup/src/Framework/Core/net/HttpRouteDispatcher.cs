using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Common;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Framework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Net
{
    class HttpRouteDispatcher {
        sealed class ThreadLocalCache : FastThreadLocal<AsciiString>
        {
            protected override AsciiString GetInitialValue()
            {
                DateTime dateTime = DateTime.UtcNow;
                return AsciiString.Cached($"{dateTime.DayOfWeek}, {dateTime:dd MMM yyyy HH:mm:ss z}");
            }
        }

        static readonly ThreadLocalCache Cache = new ThreadLocalCache();


        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static HttpRouteDispatcher Instance = new HttpRouteDispatcher();

        /** [uri, CmdExecutor] */
        private static Dictionary<string, CmdExecutor> HTTP_ROUTE_CMD_HANDLERS = new();

        


        // 头
        static readonly AsciiString ContentTypeEntity = HttpHeaderNames.ContentType;
        static readonly AsciiString ServerEntity = HttpHeaderNames.Server;
        static readonly AsciiString DateEntity = HttpHeaderNames.Date;
        static readonly AsciiString ContentLengthEntity = HttpHeaderNames.ContentLength;
        static readonly AsciiString TypePlain = AsciiString.Cached("text/plain");
        static readonly AsciiString TypeJson = AsciiString.Cached("application/json");
        // end

        // value
        volatile ICharSequence date = Cache.Value;
        static readonly AsciiString ServerName = AsciiString.Cached("x2interaction");
        // end

        public void Init()
        {
            IEnumerable<Type> controllers = TypeScanner.ListTypesWithAttribute(typeof(HttpController));
            foreach (Type controller in controllers)
            {
                try
                {
                    object handler = Activator.CreateInstance(controller);
                    MethodInfo[] methods = controller.GetMethods();

                    foreach (MethodInfo method in methods)
                    {
                        HttpRequestMapping mapperAttribute = method.GetCustomAttribute<HttpRequestMapping>();
                        if (mapperAttribute == null) {
                            continue;
                        }

                        string key = mapperAttribute.uri;
                        HTTP_ROUTE_CMD_HANDLERS.TryGetValue(key, out CmdExecutor cmdExecutor);
                        if (cmdExecutor != null)
                        {
                            logger.Warn($"Http Route[{key}] 重复注册处理器");
                            return;
                        }

                        cmdExecutor = CmdExecutor.Create(method, method.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray(), handler);
                        HTTP_ROUTE_CMD_HANDLERS.Add(key, cmdExecutor);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }
        }

        private object[] ConvertToMethodParams(IHttpRequest request, Type[] methodParams)
        {
            object[] result = new object[methodParams == null ? 0 : methodParams.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Type param = methodParams[i];
                if (param.IsAssignableTo(typeof(IHttpRequest)))
                {
                    result[i] = request;
                }
            }

            return result;
        }
        

        public void OnHttpRequrestProcess(IChannelHandlerContext ctx, IHttpRequest request) {
            string uri = request.Uri;
            if (uri.StartsWith("/Download"))
            {
                uri = "/Download";
            }

            HTTP_ROUTE_CMD_HANDLERS.TryGetValue(uri, out CmdExecutor cmdExecutor);
            if (cmdExecutor == null)
            {
                logger.Warn($"Http uri executor missed, {uri}");

                var response = new DefaultFullHttpResponse(HttpVersion.Http11, HttpResponseStatus.NotFound, Unpooled.Empty, false);
                ctx.WriteAndFlushAsync(response);
                ctx.CloseAsync();

                return;
            }

            // 目前这里暂时直接调用，不走我们的线程池Mask
            try
            {
                object[] @params = ConvertToMethodParams(request, cmdExecutor.@params);
                string response = (string)(cmdExecutor.method.Invoke(cmdExecutor.handler, @params));
                if (response != null)
                {
                    byte[] jsonBody = Encoding.UTF8.GetBytes(response);
                    WriteResponse(ctx, Unpooled.WrappedBuffer(jsonBody), TypeJson, AsciiString.Cached($"{jsonBody.Length}"));
                }
            }
            catch (Exception e)
            {
                logger.Warn("message task execute failed" + e.Message);
            }
            // end
        }

        void WriteResponse(IChannelHandlerContext ctx, IByteBuffer buf, ICharSequence contentType, ICharSequence contentLength)
        {

            // Build the response object.
            var response = new DefaultFullHttpResponse(HttpVersion.Http11, HttpResponseStatus.OK, buf, false);
            HttpHeaders headers = response.Headers;

            // 设置允许跨域 CORS 响应头
            response.Headers.Set(HttpHeaderNames.AccessControlAllowOrigin, "*");
            response.Headers.Set(HttpHeaderNames.AccessControlAllowMethods, "GET, POST, PUT, DELETE, OPTIONS");
            response.Headers.Set(HttpHeaderNames.AccessControlAllowHeaders, "Content-Type, Authorization");
            response.Headers.Set(HttpHeaderNames.AccessControlMaxAge, "3600");
            // end


            headers.Set(ContentTypeEntity, contentType); // header ---> value;
            headers.Set(ServerEntity, ServerName);
            headers.Set(DateEntity, this.date);
            headers.Set(ContentLengthEntity, contentLength);

            // Close the non-keep-alive connection after the write operation is done.
            ctx.WriteAsync(response);
        }
    }
}
