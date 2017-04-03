using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Porthor.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Porthor
{
    public class RouteBuilder
    {
        private readonly Regex _segmentSplitRegex = new Regex(@"(?={)|(?<=})");
        private readonly Regex _parameterMatchRegex = new Regex(@"(?<={).*(?=})");
        private readonly Resource _resource;

        public RouteBuilder(Resource resource)
        {
            _resource = resource;
        }

        public IRouter Build(IInlineConstraintResolver inlineConstraintResolver)
        {
            IHttpMethodStrategy strategy;

            var endpointUrlSegments = CreateEndpointUrlSegments();

            if (_resource.Method.Equals(HttpMethod.Get))
            {
                strategy = new GetStrategy(_resource.QueryParameters, _resource.ContentDefinitions, endpointUrlSegments, _resource.EndpointQueryParameters);
            }
            else if (_resource.Method.Equals(HttpMethod.Post))
            {
                strategy = new PostStrategy(_resource.QueryParameters, _resource.ContentDefinitions, endpointUrlSegments, _resource.EndpointQueryParameters);
            }
            else if (_resource.Method.Equals(HttpMethod.Put))
            {
                strategy = new PutStrategy(_resource.QueryParameters, _resource.ContentDefinitions, endpointUrlSegments, _resource.EndpointQueryParameters);
            }
            else if (_resource.Method.Equals(HttpMethod.Delete))
            {
                strategy = new DeleteStrategy(_resource.QueryParameters, _resource.ContentDefinitions, endpointUrlSegments, _resource.EndpointQueryParameters);
            }
            else
            {
                throw new NotSupportedException(_resource.Method.Method);
            }

            return new Route(
                new RouteHandler(strategy.HandleRouteAsync),
                _resource.Path,
                defaults: null,
                constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint(_resource.Method.Method) }),
                dataTokens: null,
                inlineConstraintResolver: inlineConstraintResolver);
        }

        private IEnumerable<IEndpointUrlSegment> CreateEndpointUrlSegments()
        {
            List<IEndpointUrlSegment> segments = new List<IEndpointUrlSegment>();

            string[] urlSegments = _segmentSplitRegex.Split(_resource.EndpointUrl);
            for (int i = 0; i < urlSegments.Length; i++)
            {
                var parameterMatch = _parameterMatchRegex.Match(urlSegments[i]);
                if (parameterMatch.Success)
                {
                    segments.Add(new ParameterUrlSegment(parameterMatch.Value));
                }
                else
                {
                    segments.Add(new StringUrlSegment(urlSegments[i]));
                }
            }

            return segments;
        }
    }
}
