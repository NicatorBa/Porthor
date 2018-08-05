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
        /// Add a custom <see cref="HttpMessageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">Customer <see cref="HttpMessageHandler"/>.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder AddMessageHandler(HttpMessageHandler messageHandler);

        /// <summary>
        /// Add the validation for authentication.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder AddAuthenticationValidation(bool enabled = true);

        /// <summary>
        /// Add the validation for authorization.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder AddAuthorizationValidation(bool enabled = true);

        /// <summary>
        /// Add the validation of query strings.
        /// </summary>
        /// <param name="enabled">A value indicating whether the validation is enabled.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder AddQueryStringValidation(bool enabled = true);

        /// <summary>
        /// Add the validation of content.
        /// </summary>
        /// <param name="options">The action used to configure the content options.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        IPorthorBuilder AddContentValidation(Action<ContentOptions> options);
    }
}
