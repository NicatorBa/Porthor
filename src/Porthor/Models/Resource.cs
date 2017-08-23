using System.Collections.Generic;
using System.Net.Http;

namespace Porthor.Models
{
    /// <summary>
    /// Represents an API endpoint for the gateway with all necessary settings.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Name for the resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Optional description for the API endpoint.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Supported HTTP method.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Path to access the API endpoint from your application.
        /// </summary>
        /// <example>
        /// var resource = new Resource();
        /// resource.Path = "api/v1/samples";
        /// </example>
        public string Path { get; set; }

        /// <summary>
        /// Authentication and authorization settings for this resource.
        /// </summary>
        public SecuritySettings SecuritySettings { get; set; }

        /// <summary>
        /// Settings how to handle the query string of the resource.
        /// </summary>
        public QueryParameterSettings QueryParameterSettings { get; set; }

        /// <summary>
        /// Request content definitions for POST or PUT method.
        /// </summary>
        public ICollection<ContentDefinition> ContentDefinitions { get; set; }

        /// <summary>
        /// Distributed API endpoint to which the request is sent.
        /// </summary>
        public string EndpointUrl { get; set; }
    }
}
