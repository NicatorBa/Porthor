using Porthor.Configuration;
using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Helper functions for configuring gateway services.
    /// </summary>
    public interface IPorthorBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> services are attached to.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Add a custom <see cref="HttpMessageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">Customer <see cref="HttpMessageHandler"/>.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder AddMessageHandler(HttpMessageHandler messageHandler);

        /// <summary>
        /// Enable the validation for authentication.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder EnableAuthenticationValidation(bool enabled = true);

        /// <summary>
        /// Enable the validation for authorization.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder EnableAuthorizationValidation(bool enabled = true);

        /// <summary>
        /// Enable the validation of query strings.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder EnableQueryStringValidation(bool enabled = true);

        /// <summary>
        /// Configure the validation of content.
        /// </summary>
        /// <param name="options">The action used to configure the content options.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder ConfigureContentValidation(Action<ContentOptions> options);
    }
}
