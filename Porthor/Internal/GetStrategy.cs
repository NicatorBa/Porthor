using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Porthor.Internal
{
    internal class GetStrategy : BaseHttpMethodStrategy
    {
        public GetStrategy(
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

            var resultMessage = await url.GetAsync();

            await CreateHttpResponse(response, resultMessage);
        }
    }
}
