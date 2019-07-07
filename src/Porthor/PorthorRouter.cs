using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Porthor.Configuration;
using Porthor.Internal;
using Porthor.Models;
using Porthor.Validation;

namespace Porthor
{
    /// <summary>
    /// Represents a router.
    /// </summary>
    public class PorthorRouter : IPorthorRouter
    {
        private readonly IInlineConstraintResolver _constraintResolver;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        private readonly MessageHandlerOptions _messageHandlerOptions;
        private readonly AuthenticationOptions _authenticationOptions;
        private readonly AuthorizationOptions _authorizationOptions;
        private readonly QueryStringOptions _queryStringOptions;
        private readonly ContentOptions _contentOptions;

        private IRouter _router;

        /// <summary>
        /// Constructs a new instance of <see cref="PorthorRouter"/>.
        /// </summary>
        /// <param name="constraintResolver">Resolver for inline constraints.</param>
        /// <param name="configuration">Configuration of application properties.</param>
        /// <param name="logger">Logger for <see cref="PorthorRouter"/>.</param>
        /// <param name="messageHandlerOptionsAccessor">Accessor for message handler options.</param>
        /// <param name="authenticationOptionsAccessor">Accessor for authentication options.</param>
        /// <param name="authorizationOptionsAccessor">Accessor for authorization options.</param>
        /// <param name="queryStringOptionsAccessor">Accessor for query string options.</param>
        /// <param name="contentOptionsAccessor">Accessor for content options.</param>
        public PorthorRouter(
            IInlineConstraintResolver constraintResolver,
            IConfiguration configuration,
            ILogger<PorthorRouter> logger,
            IOptions<MessageHandlerOptions> messageHandlerOptionsAccessor,
            IOptions<AuthenticationOptions> authenticationOptionsAccessor,
            IOptions<AuthorizationOptions> authorizationOptionsAccessor,
            IOptions<QueryStringOptions> queryStringOptionsAccessor,
            IOptions<ContentOptions> contentOptionsAccessor)
        {
            _constraintResolver = constraintResolver;
            _configuration = configuration;
            _logger = logger;

            _messageHandlerOptions = messageHandlerOptionsAccessor.Value;
            _authenticationOptions = authenticationOptionsAccessor.Value;
            _authorizationOptions = authorizationOptionsAccessor.Value;
            _queryStringOptions = queryStringOptionsAccessor.Value;
            _contentOptions = contentOptionsAccessor.Value;

            _router = new RouteCollection();
        }

        /// <inheritdoc />
        public Task InitializeAsync(IEnumerable<RoutingRule> rules)
        {
            var routeCollection = new RouteCollection();

            foreach (var rule in rules)
            {
                var validators = new List<IValidator>();

                if (_authenticationOptions.Enabled &&
                    (rule.ValidationSettings?.Authentication == null ||
                    !rule.ValidationSettings.Authentication.AllowAnonymous))
                {
                    validators.Add(new AuthenticationValidator());
                }

                if (_authorizationOptions.Enabled &&
                    rule.ValidationSettings?.Authorization?.Policies != null)
                {
                    validators.Add(new AuthorizationValidator(rule.ValidationSettings.Authorization.Policies));
                }

                if (_queryStringOptions.Enabled)
                {
                    var queryString = rule.ValidationSettings?.QueryString ?? new QueryString();
                    queryString.QueryParameters = queryString.QueryParameters ?? new QueryParameter[] { };

                    validators.Add(new QueryStringValidator(queryString));
                }

                if (_contentOptions.Enabled &&
                    rule.ValidationSettings?.Contents != null &&
                    rule.ValidationSettings.Contents.Any())
                {
                    validators.Add(new MediaTypeContentValidator(_contentOptions, rule.ValidationSettings.Contents));
                }

                var requestHandler = new RequestHandler(
                    RequestUriBuilder.Initialize(rule.BackendUrl, _configuration),
                    _messageHandlerOptions.MessageHandler,
                    rule.Timeout.HasValue ? TimeSpan.FromSeconds(rule.Timeout.Value) : (TimeSpan?)null,
                    validators);
                var route = new Route(
                    new RouteHandler(requestHandler.HandleAsync),
                    rule.FrontendPath,
                    null,
                    new RouteValueDictionary(new { httpMethod = new HttpMethodRouteConstraint(rule.HttpMethod.Method) }),
                    null,
                    _constraintResolver);

                routeCollection.Add(route);
            }

            _logger.LogInformation("Initialized router with {count} rules.", routeCollection.Count);

            _router = routeCollection;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _router.GetVirtualPath(context);
        }

        /// <inheritdoc />
        public Task RouteAsync(RouteContext context)
        {
            return _router.RouteAsync(context);
        }
    }
}
