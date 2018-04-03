using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;
using Porthor.EndpointUri;
using Porthor.Models;
using Porthor.ResourceRequestValidators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porthor
{
    /// <summary>
    /// Represents a router.
    /// </summary>
    public class PorthorRouter : IPorthorRouter
    {
        private readonly IInlineConstraintResolver _contraintResolver;
        private readonly PorthorOptions _options;
        private IRouter _router;

        /// <summary>
        /// Constructs a new instance of <see cref="PorthorRouter"/>.
        /// </summary>
        /// <param name="constraintResolver">Resolver for inline constraints.</param>
        /// <param name="options">Configuration options.</param>
        public PorthorRouter(
            IInlineConstraintResolver constraintResolver,
            IOptions<PorthorOptions> options)
        {
            _contraintResolver = constraintResolver;
            _options = options.Value;
            _router = new RouteCollection();
        }

        /// <summary>
        /// Get <see cref="VirtualPathData"/> from <see cref="VirtualPathContext"/>.
        /// </summary>
        /// <param name="context">A context for virtual path generation operations.</param>
        /// <returns>Information about the route and virtual path.</returns>
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _router.GetVirtualPath(context);
        }

        /// <summary>
        /// Initialize the router with the specified resources.
        /// </summary>
        /// <param name="resources">Collection of API resources.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous initialization process.</returns>
        public Task Initialize(IEnumerable<Resource> resources)
        {
            var routeCollection = new RouteCollection();

            foreach (var resource in resources)
            {
                var validators = new List<IResourceRequestValidator>();

                if (_options.QueryStringValidationEnabled)
                {
                    if (resource.QueryParameterSettings == null)
                    {
                        resource.QueryParameterSettings = new QueryParameterSettings();
                    }

                    if (resource.QueryParameterSettings.QueryParameters == null)
                    {
                        resource.QueryParameterSettings.QueryParameters = new List<QueryParameter>();
                    }

                    validators.Add(new QueryParameterValidator(resource.QueryParameterSettings));
                }

                if (_options.Security.AuthenticationValidationEnabled &&
                    (resource.SecuritySettings == null ||
                    !resource.SecuritySettings.AllowAnonymous))
                {
                    validators.Add(new AuthenticationValidator());
                }

                if (_options.Security.AuthorizationValidationEnabled &&
                    resource.SecuritySettings?.Policies != null)
                {
                    validators.Add(new AuthorizationValidator(resource.SecuritySettings.Policies));
                }

                if (_options.Content.ValidationEnabled &&
                    resource.ContentDefinitions != null &&
                    resource.ContentDefinitions.Count > 0)
                {
                    validators.Add(new ContentDefinitionValidator(resource.ContentDefinitions, _options.Content));
                }

                var resourceHandler = new ResourceHandler(
                    validators,
                    EndpointUriBuilder.Initialize(resource.EndpointUrl, _options.Configuration),
                    resource.Timeout.HasValue ? TimeSpan.FromSeconds(resource.Timeout.Value) : (TimeSpan?)null,
                    _options.BackChannelMessageHandler);
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

        /// <summary>
        /// Route the current context.
        /// </summary>
        /// <param name="context">A context object for Microsoft.AspNetCore.Routing.IRouter.RouteAsync(Microsoft.AspNetCore.Routing.RouteContext).</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous routing process.</returns>
        public Task RouteAsync(RouteContext context)
        {
            return _router.RouteAsync(context);
        }
    }
}
