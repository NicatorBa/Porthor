using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.Internal
{
    internal abstract class BaseHttpMethodStrategy : IHttpMethodStrategy
    {
        public BaseHttpMethodStrategy(
            IEnumerable<ResourceQueryParameter> resourceQueryParameters,
            ICollection<ContentDefinition> contentDefinitions,
            IEnumerable<IEndpointUrlSegment> endpointUrlSegments,
            IEnumerable<EndpointQueryParameter> endpointQueryParameters)
        {
            ResourceQueryParameters = resourceQueryParameters;
            ContentDefinitions = contentDefinitions;
            EndpointUrlSegments = endpointUrlSegments;
            EndpointQueryParameters = endpointQueryParameters;
        }

        private IEnumerable<ResourceQueryParameter> ResourceQueryParameters { get; set; }

        public ICollection<ContentDefinition> ContentDefinitions { get; set; }

        protected IEnumerable<IEndpointUrlSegment> EndpointUrlSegments { get; private set; }

        protected IEnumerable<EndpointQueryParameter> EndpointQueryParameters { get; set; }

        public Task HandleRouteAsync(HttpContext context)
        {
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                foreach (var routeParameter in context.GetRouteData().Values)
                {
                    parameters.Add(routeParameter.Key, routeParameter.Value.ToString());
                }
                foreach (var queryParameter in context.Request.Query)
                {
                    var resourceQueryParameter = ResourceQueryParameters.SingleOrDefault(p => p.Field.Equals(queryParameter.Key));
                    if (resourceQueryParameter != null)
                    {
                        if (resourceQueryParameter.Required &&
                            string.IsNullOrWhiteSpace(queryParameter.Value))
                        {
                            throw new ArgumentNullException(queryParameter.Key);
                        }

                        parameters.Add(queryParameter.Key, queryParameter.Value);
                    }
                }

                return HandleAsync(context.Request, parameters, context.Response);
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Task.CompletedTask;
            }
        }

        protected abstract Task HandleAsync(HttpRequest request, IDictionary<string, string> parameters, HttpResponse response);

        protected async Task CreateHttpResponse(HttpResponse response, HttpResponseMessage message)
        {
            response.StatusCode = (int)message.StatusCode;
            response.ContentType = message.Content.Headers.ContentType.MediaType;
            byte[] content = await message.Content.ReadAsByteArrayAsync();
            await response.Body.WriteAsync(content, 0, content.Length);
        }
    }
}
