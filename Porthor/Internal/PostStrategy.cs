using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using NJsonSchema;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Porthor.Internal
{
    internal class PostStrategy : BaseHttpMethodStrategy
    {
        public PostStrategy(
            IEnumerable<ResourceQueryParameter> resourceQueryParameters,
            ICollection<ContentDefinition> contentDefinitions,
            IEnumerable<IEndpointUrlSegment> endpointUrlSegments,
            IEnumerable<EndpointQueryParameter> endpointQueryParameters)
            : base(resourceQueryParameters, contentDefinitions, endpointUrlSegments, endpointQueryParameters)
        { }

        protected override async Task HandleAsync(HttpRequest request, IDictionary<string, string> parameters, HttpResponse response)
        {
            var urlBuilder = new StringBuilder();
            foreach (var urlSegment in EndpointUrlSegments)
            {
                urlBuilder.Append(urlSegment.GetSegment(parameters));
            }
            string url = urlBuilder.ToString();
            foreach (var queryParameter in EndpointQueryParameters)
            {
                var queryParameterValue = parameters[queryParameter.ValueKey];
                if (queryParameterValue != null)
                {
                    url.SetQueryParam(queryParameter.Field, queryParameterValue, true);
                }
            }

            var contentDefinition = ContentDefinitions.SingleOrDefault(c => c.ContentType.Equals(request.ContentType));
            if (contentDefinition == null)
            {
                response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
            }

            if (contentDefinition.ContentSchema != null)
            {
                if (contentDefinition.ContentType.Equals(ContentType.Json))
                {
                    var jsonSchema = await JsonSchema4.FromJsonAsync(contentDefinition.ContentSchema);
                    var errors = jsonSchema.Validate(await StreamToString(request.Body));
                    if (errors.Count > 0)
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return;
                    }
                }
            }

            var body = new StreamContent(request.Body);
            var resultMessage = await url.PostAsync(body);

            await CreateHttpResponse(response, resultMessage);
        }

        private Task<string> StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEndAsync();
            }
        }
    }
}
