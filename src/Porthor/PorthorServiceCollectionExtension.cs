using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Porthor;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding the porthor services to an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class PorthorServiceCollectionExtension
    {
        /// <summary>
        /// Registers the required services for <see cref="AspNetCore.Builder.PorthorMiddleware"/>.
        /// </summary>
        /// <param name="services">Contract for a collection of service descriptors.</param>
        public static void AddPorthor(
            this IServiceCollection services)
        {
            services.AddPorthor(options => { });
        }

        /// <summary>
        /// Registers the required services for <see cref="AspNetCore.Builder.PorthorMiddleware"/>.
        /// </summary>
        /// <param name="services">Contract for a collection of service descriptors.</param>
        /// <param name="setupAction">An action to configure the <see cref="PorthorOptions"/> for the <see cref="AspNetCore.Builder.PorthorMiddleware"/>.</param>
        public static void AddPorthor(
            this IServiceCollection services,
            Action<PorthorOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.TryAddTransient<IInlineConstraintResolver, DefaultInlineConstraintResolver>();
            services.TryAddSingleton<IPorthorRouter, PorthorRouter>();

            services.Configure(setupAction);
        }
    }
}
