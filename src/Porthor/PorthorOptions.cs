using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Porthor
{
    /// <summary>
    /// Configuration options for <see cref="Microsoft.AspNetCore.Builder.PorthorMiddleware"/>.
    /// </summary>
    public class PorthorOptions
    {
        /// <summary>
        /// HTTP message handler.
        /// </summary>
        public HttpMessageHandler BackChannelMessageHandler { get; set; }

        /// <summary>
        /// Application configuration properties.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Flag that indicates whether the query string is to be validated.
        /// </summary>
        public bool QueryStringValidationEnabled { get; set; }

        /// <summary>
        /// Security configuration options.
        /// </summary>
        public SecurityOptions Security { get; } = new SecurityOptions();

        /// <summary>
        /// Request content configuration options.
        /// </summary>
        public ContentOptions Content { get; } = new ContentOptions();
    }
}
