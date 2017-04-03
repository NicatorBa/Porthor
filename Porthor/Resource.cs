using System.Collections.Generic;
using System.Net.Http;

namespace Porthor
{
    /// <summary>
    /// Represents an API resource in the <see cref="PorthorMiddleware"/>.
    /// </summary>
    public class Resource
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public HttpMethod Method { get; set; }

        public string Path { get; set; }

        public ICollection<ResourceQueryParameter> QueryParameters { get; set; }

        public ICollection<ContentDefinition> ContentDefinitions { get; set; }

        public string EndpointUrl { get; set; }

        public ICollection<EndpointQueryParameter> EndpointQueryParameters { get; set; }
    }
}
