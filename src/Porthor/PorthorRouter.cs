using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Porthor.EndpointUri;
using Porthor.Models;
using Porthor.ResourceRequestValidators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porthor
{
    public class PorthorRouter : IPorthorRouter
    {
        private readonly IInlineConstraintResolver _contraintResolver;
        private readonly IConfiguration _config;
        private readonly PorthorOptions _options;
        private IRouter _router;

        public PorthorRouter(
            IInlineConstraintResolver constraintResolver,
            IConfiguration config,
            IOptions<PorthorOptions> options)
        {
            _contraintResolver = constraintResolver;
            _config = config;
            _options = options.Value;
            _router = new RouteCollection();
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _router.GetVirtualPath(context);
        }

        public Task Initialize(IEnumerable<Resource> resources)
        {
            var routeCollection = new RouteCollection();

            foreach (var resource in resources)
            {
                var validators = new List<IResourceRequestValidator>();

                if (_options.QueryStringValidationEnabled)
                {
                    validators.Add(new QueryParameterValidator(resource.QueryParameterSettings));
                }

                if (_options.Security.AuthenticationValidationEnabled &&
                    !resource.SecuritySettings.AllowAnonymous)
                {
                    validators.Add(new AuthenticationValidator());
                }

                if (_options.Security.AuthorizationValidationEnabled)
                {
                    validators.Add(new AuthorizationValidator(resource.SecuritySettings.Policies));
                }

                if (_options.Content.ValidationEnabled)
                {
                    validators.Add(new ContentDefinitionValidator(resource.ContentDefinitions, _options.Content));
                }

                var resourceHandler = new ResourceHandler(
                    validators,
                    EndpointUriBuilder.Initialize(resource.EndpointUrl, _config));
                var route = new Route(
                    new RouteHandler(resourceHandler.HandleRequestAsync),
                    resource.Path,
                    null,
                    new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint(resource.Method.Method) }),
                    null,
                    _contraintResolver);
                routeCollection.Add(route);
            }

            _router = routeCollection;

            return Task.CompletedTask;
        }

        public Task RouteAsync(RouteContext context)
        {
            return _router.RouteAsync(context);
        }
    }
}
