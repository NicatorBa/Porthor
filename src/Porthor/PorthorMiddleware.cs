using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Porthor;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// A middleware that handle requests and route to the backend APIs.
    /// </summary>
    public class PorthorMiddleware : RouterMiddleware
    {
        /// <summary>
        /// Constructs a new instance of <see cref="PorthorMiddleware"/>.
        /// </summary>
        /// <param name="next">Function that can process an HTTP request.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        /// <param name="router">A router.</param>
        public PorthorMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IPorthorRouter router)
            : base(next, loggerFactory, router)
        { }
    }
}
