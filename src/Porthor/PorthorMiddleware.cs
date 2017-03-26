using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Porthor
{
    public class PorthorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public PorthorMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<PorthorMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext, IPorthorRouter router)
        {
            var context = new RouteContext(httpContext);
            context.RouteData.Routers.Add(router);

            await router.RouteAsync(context);

            if (context.Handler == null)
            {
                await _next.Invoke(httpContext);
            }
            else
            {
                httpContext.Features[typeof(IRoutingFeature)] = new RoutingFeature()
                {
                    RouteData = context.RouteData
                };

                await context.Handler(context.HttpContext);
            }
        }
    }
}
