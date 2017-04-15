using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;
using Porthor.ContentValidation;
using Porthor.EndpointUri;
using Porthor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porthor
{
    public class PorthorRouter : IPorthorRouter
    {
        private readonly IInlineConstraintResolver _constraintResolver;
        private readonly PorthorOptions _options;
        private IRouter _router;

        public PorthorRouter(IInlineConstraintResolver constraintResolver, IOptions<PorthorOptions> options)
        {
            _constraintResolver = constraintResolver;
            _options = options.Value;
            _router = new RouteCollection();
        }

        public async Task Build(IEnumerable<Resource> resources)
        {
            var routeCollection = new RouteCollection();
            var defaultContentValidator = new DefaultContentValidator();
            foreach (var resource in resources)
            {
                IDictionary<string, IContentValidator> mediaTypeContentValidators = new Dictionary<string, IContentValidator>();
                foreach (var contentDefinition in resource.ContentDefinitions)
                {
                    if (_options.ContentValidatorFactories.ContainsKey(contentDefinition.MediaType) ||
                        string.IsNullOrWhiteSpace(contentDefinition.Template))
                    {
                        var contentValidatorFactory = _options.ContentValidatorFactories[contentDefinition.MediaType];
                        mediaTypeContentValidators.Add(contentDefinition.MediaType, await contentValidatorFactory.CreateContentValidatorAsync(contentDefinition.Template));
                    }
                    else
                    {
                        mediaTypeContentValidators.Add(contentDefinition.MediaType, defaultContentValidator);
                    }
                }

                var endpointUriFactory = EndpointUriFactory.Initialize(resource);
                var resourceHandler = new ResourceHandler(resource.Method, resource.QueryParameterConfiguration, mediaTypeContentValidators, endpointUriFactory);
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
