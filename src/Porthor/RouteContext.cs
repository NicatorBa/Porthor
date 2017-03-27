using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Porthor
{
    public class RouteContext
    {
        private readonly Regex _splitRegex = new Regex(@"(?={)|(?<=})");
        private readonly Regex _matchRegex = new Regex(@"(?<={).*(?=})");

        private readonly Resource _resource;

        public RouteContext(Resource resource)
        {
            _resource = resource;
        }

        public IRouter Build(IInlineConstraintResolver inlineContraintReslover)
        {
            return new Route(
                new RouteHandler(HandleRequestAsync),
                _resource.Path,
                defaults: null,
                constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint(_resource.Method.Method) }),
                dataTokens: null,
                inlineConstraintResolver: inlineContraintReslover);
        }

        private async Task HandleRequestAsync(HttpContext context)
        {
            try
            {
                string[] endpointUrlParts = _splitRegex.Split(_resource.EndpointUrl);

                var endpointUrlBuilder = new StringBuilder();
                for (int i = 0; i < endpointUrlParts.Length; i++)
                {
                    var routeParameter = _matchRegex.Match(endpointUrlParts[i]);
                    if (routeParameter.Success)
                    {
                        endpointUrlBuilder.Append(context.GetRouteValue(routeParameter.Value));
                    }
                    else
                    {
                        endpointUrlBuilder.Append(endpointUrlParts[i]);
                    }
                }
                var endpointUrl = endpointUrlBuilder.ToString();

                if (_resource.Method.Equals(HttpMethod.Get))
                {
                    var result = await endpointUrl.GetAsync();
                    context.Response.StatusCode = (int)result.StatusCode;
                    context.Response.Body = await result.Content.ReadAsStreamAsync();
                    context.Response.ContentType = result.Content.Headers.ContentType.MediaType;
                    return;
                }

                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
