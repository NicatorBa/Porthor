using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Porthor
{
    public static class PorthorServiceCollectionExtension
    {
        public static IServiceCollection AddPorthor(
            this IServiceCollection services,
            Action<PorthorOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.TryAddTransient<IInlineConstraintResolver, DefaultInlineConstraintResolver>();
            services.TryAddTransient<IPorthorRouter, PorthorRouter>();

            services.Configure(setupAction);

            return services;
        }
    }
}
