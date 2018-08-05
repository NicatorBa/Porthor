using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Porthor.Internal
{
    /// <summary>
    /// Builder for a specific request uri.
    /// </summary>
    public class RequestUriBuilder
    {
        private readonly IRequestUriPartAccessor[] _requestUriPartAccessors;

        private RequestUriBuilder(IRequestUriPartAccessor[] requestUriPartAccessors)
        {
            _requestUriPartAccessors = requestUriPartAccessors;
        }

        /// <summary>
        /// Initialize an instance of <see cref="RequestUriBuilder"/>.
        /// </summary>
        /// <param name="uriTemplate">The template for the uri.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <returns>A new instance of <see cref="RequestUriBuilder"/>.</returns>
        public static RequestUriBuilder Initialize(string uriTemplate, IConfiguration configuration)
        {
            var accessors = new List<IRequestUriPartAccessor>();

            var splitRegex = new Regex(@"(?=[{[])|(?<=[}\]])");
            var matchEnvRegex = new Regex(@"(?<=\[).*(?=])");
            var matchRouteRegex = new Regex(@"(?<={).*(?=})");

            var parts = splitRegex.Split(uriTemplate);
            foreach (var part in parts)
            {
                var envMatch = matchEnvRegex.Match(part);
                var routeMatch = matchRouteRegex.Match(part);

                if (routeMatch.Success)
                {
                    accessors.Add(new RouteValueUriPartAccessor(routeMatch.Value));
                }
                else
                {
                    var value = envMatch.Success ? configuration[envMatch.Value] : part;

                    if (accessors.Any() && accessors.Last() is StaticUriPartAccessor)
                    {
                        var lastAccessor = (StaticUriPartAccessor)accessors.Last();
                        accessors.Remove(lastAccessor);
                        accessors.Add(lastAccessor.Extend(value));
                    }
                    else
                    {
                        accessors.Add(new StaticUriPartAccessor(value));
                    }
                }
            }

            return new RequestUriBuilder(accessors.ToArray());
        }

        /// <summary>
        /// Build the request uri for the current context.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <returns>The <see cref="Uri"/> of an API endpoint.</returns>
        public Uri Build(HttpContext context)
        {
            var builder = new StringBuilder();

            var routeValues = context.GetRouteData().Values;
            for (int i = 0; i < _requestUriPartAccessors.Length; i++)
            {
                builder.Append(_requestUriPartAccessors[i].GetUriPart(routeValues));
            }

            if (context.Request.QueryString.HasValue)
            {
                builder.Append(context.Request.QueryString.Value);
            }

            return new Uri(builder.ToString());
        }
    }
}
