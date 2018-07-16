using System.Net.Http;

namespace Porthor.Configuration
{
    /// <summary>
    /// Message handler options.
    /// </summary>
    public class MessageHandlerOptions
    {
        /// <summary>
        /// Gets or sets the message handler.
        /// </summary>
        public HttpMessageHandler MessageHandler { get; set; }
    }
}
