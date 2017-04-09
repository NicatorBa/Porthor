using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Porthor.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Porthor.EndpointUri
{
    public class EndpointUriFactory
    {
        private readonly IEnumerable<IEndpointUriSegment> _uriSegments;

        private EndpointUriFactory(IEnumerable<IEndpointUriSegment> uriSegments)
        {
            _uriSegments = uriSegments;
        }

        public static EndpointUriFactory Initialize(Resource resource)
        {
            var splitRegex = new Regex(@"(?={)|(?<=})");
            var matchRegex = new Regex(@"(?<={).*(?=})");

            var uriSegments = new List<IEndpointUriSegment>();
            string[] segments = splitRegex.Split(resource.EndpointUrl);
            for (int i = 0; i < segments.Length; i++)
            {
                var parameterMatch = matchRegex.Match(segments[i]);
                if (parameterMatch.Success)
                {
                    uriSegments.Add(new ParameterUriSegment(parameterMatch.Value));
                }
                else
                {
                    uriSegments.Add(new ConstantUriSegment(segments[i]));
                }
            }

            return new EndpointUriFactory(uriSegments);
        }

        public Uri CreateUri(HttpContext context)
        {
            var uriBuilder = new StringBuilder();

            var routeValues = context.GetRouteData().Values;
            foreach (var uriSegment in _uriSegments)
            {
                uriBuilder.Append(uriSegment.GetSegment(routeValues));
            }

            if (context.Request.QueryString.HasValue)
            {
                uriBuilder.Append(context.Request.QueryString.Value);
            }

            return new Uri(uriBuilder.ToString());
        }
    }
}
