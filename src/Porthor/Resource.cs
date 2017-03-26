using System.Net.Http;

namespace Porthor
{
    /// <summary>
    /// Represents an API resource in the porthor gateway.
    /// </summary>
    public class Resource
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Path { get; set; }

        public HttpMethod Method { get; set; }

        public string Accept { get; set; }

        public string Schema { get; set; }

        public string EndpointUrl { get; set; }
    }
}
