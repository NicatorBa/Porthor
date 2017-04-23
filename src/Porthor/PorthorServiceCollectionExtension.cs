using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Porthor
{
    public static class PorthorServiceCollectionExtension
    {
        public static IServiceCollection AddPorthor(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<PorthorOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.TryAddTransient<IInlineConstraintResolver, DefaultInlineConstraintResolver>();
            services.TryAddTransient<IPorthorRouter, PorthorRouter>();

            services.TryAddSingleton(configuration);

            services.Configure(setupAction);

            return services;
        }
    }
}
