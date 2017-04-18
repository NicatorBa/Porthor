using System.Collections.Generic;
using System.Net.Http;

namespace Porthor.Models
{
    public class Resource
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public HttpMethod Method { get; set; }

        public string Path { get; set; }

        public SecuritySettings SecuritySettings { get; set; }

        public QueryParameterSettings QueryParameterSettings { get; set; }

        public ICollection<ContentDefinition> ContentDefinitions { get; set; }

        public string EndpointUrl { get; set; }
    }
}
