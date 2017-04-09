using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Porthor.EndpointUri;
using Porthor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porthor
{
    public class PorthorRouter : IPorthorRouter
    {
        private readonly IInlineConstraintResolver _constraintResolver;
        private IRouter _router;

        public PorthorRouter(IInlineConstraintResolver constraintResolver)
        {
            _constraintResolver = constraintResolver;
            _router = new RouteCollection();
        }

        public Task Build(IEnumerable<Resource> resources)
        {
            var routeCollection = new RouteCollection();
            foreach (var resource in resources)
            {
                var endpointUriFactory = EndpointUriFactory.Initialize(resource);
                var resourceHandler = new ResourceHandler(resource.Method, resource.QueryParameterConfiguration, endpointUriFactory);
                var route = new Route(
                    new RouteHandler(resourceHandler.HandleRequestAsync),
                    resource.Path,
                    defaults: null,
                    constraints: new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint(resource.Method.Method) }),
                    dataTokens: null,
                    inlineConstraintResolver: _constraintResolver);
                routeCollection.Add(route);
            }
            _router = routeCollection;

            return Task.CompletedTask;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _router.GetVirtualPath(context);
        }

        public Task RouteAsync(RouteContext context)
        {
            return _router.RouteAsync(context);
        }
    }
}
