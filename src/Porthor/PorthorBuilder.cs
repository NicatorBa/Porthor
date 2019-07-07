using System;
using System.Net.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Porthor;
using Porthor.Configuration;

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

        /// <inheritdoc />
        public IPorthorBuilder AddMessageHandler(HttpMessageHandler messageHandler)
        {
            Services.Configure<MessageHandlerOptions>(options => options.MessageHandler = messageHandler);

            return this;
        }

        /// <inheritdoc />
        public IPorthorBuilder AddAuthenticationValidation(bool enabled = true)
        {
            Services.Configure<AuthenticationOptions>(options => options.Enabled = enabled);

            return this;
        }


        /// <inheritdoc />
        public IPorthorBuilder AddAuthorizationValidation(bool enabled = true)
        {
            Services.Configure<AuthorizationOptions>(options => options.Enabled = enabled);

            return this;
        }

        /// <inheritdoc />
        public IPorthorBuilder AddQueryStringValidation(bool enabled = true)
        {
            Services.Configure<QueryStringOptions>(options => options.Enabled = enabled);

            return this;
        }

        /// <inheritdoc />
        public IPorthorBuilder AddContentValidation(Action<ContentOptions> options)
        {
            Services.Configure(options);

            return this;
        }
    }
}
