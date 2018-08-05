using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Porthor;
using Porthor.Configuration;
using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Helper functions for configuring gateway services.
    /// </summary>
    public class PorthorBuilder : IPorthorBuilder
    {
        /// <summary>
        /// Creates a new instance of <see cref="PorthorBuilder"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to attach to.</param>
        public PorthorBuilder(IServiceCollection services)
        {
            services.TryAddTransient<IInlineConstraintResolver, DefaultInlineConstraintResolver>();
            services.TryAddSingleton<IPorthorRouter, PorthorRouter>();

            Services = services;
        }

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        private IServiceCollection Services { get; }

        /// <summary>
        /// Add a custom <see cref="HttpMessageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">Customer <see cref="HttpMessageHandler"/>.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        public IPorthorBuilder AddMessageHandler(HttpMessageHandler messageHandler)
        {
            Services.Configure<MessageHandlerOptions>(options => options.MessageHandler = messageHandler);

            return this;
        }

        /// <summary>
        /// Add the validation for authentication.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        public IPorthorBuilder AddAuthenticationValidation(bool enabled = true)
        {
            Services.Configure<AuthenticationOptions>(options => options.Enabled = enabled);

            return this;
        }

        /// <summary>
        /// Add the validation for authorization.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        public IPorthorBuilder AddAuthorizationValidation(bool enabled = true)
        {
            Services.Configure<AuthorizationOptions>(options => options.Enabled = enabled);

            return this;
        }

        /// <summary>
        /// Add the validation of query strings.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        public IPorthorBuilder AddQueryStringValidation(bool enabled = true)
        {
            Services.Configure<QueryStringOptions>(options => options.Enabled = enabled);

            return this;
        }

        /// <summary>
        /// Add the validation of content.
        /// </summary>
        /// <param name="options">The action used to configure the content options.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        public IPorthorBuilder AddContentValidation(Action<ContentOptions> options)
        {
            Services.Configure(options);

            return this;
        }
    }
}
