using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Porthor;

namespace Microsoft.AspNetCore.Builder
{
    public class PorthorMiddleware : RouterMiddleware
    {
        public PorthorMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IPorthorRouter router)
            : base(next, loggerFactory, router)
        { }
    }
}
