using DotNetty.Codecs.Http;
using Framework.Core.Utils;
using Game.Core.Entry.Modules;


namespace Game.Core.Entry.HttpControllers
{
    [HttpController]
    class HttpGameController
    {
        [HttpRequestMapping("/test")]
        public string DoTestAction(IHttpRequest request)
        {
            // Do something
            // end

            return "DoTestAction";
        }

        [HttpRequestMapping("/Login")]
        public string DoLoginAction(IHttpRequest request)
        {
            // Do something
            // end

            return "DoLoginAction";
        }

        [HttpRequestMapping("/PullingServerZone")]
        public string DoPullServerZoneAction(IHttpRequest request)
        {
            return HttpServerZoneModule.Instance.HandlePullServerZoneAction();
        }

    }
}
