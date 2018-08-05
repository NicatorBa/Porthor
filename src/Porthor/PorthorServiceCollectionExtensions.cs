using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding the porthor services to an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class PorthorServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the required services for <see cref="AspNetCore.Builder.PorthorMiddleware"/>.
        /// </summary>
        /// <param name="services">Contract for a collection of service descriptors.</param>
        /// <returns>Helper for configuring gateway services.</returns>
        public static IPorthorBuilder AddPorthor(this IServiceCollection services)
        {
            var builder = new PorthorBuilder(services);

            builder
                .AddMessageHandler(new HttpClientHandler())
                .AddAuthenticationValidation(false)
                .AddAuthorizationValidation(false)
                .AddQueryStringValidation(false)
                .AddContentValidation(options => options.Enabled = false);

            return builder;
        }
    }
}
