using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Porthor;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PorthorServiceCollectionExtension
    {
        public static void AddPorthor(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<PorthorOptions> setupAction)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.TryAddTransient<IInlineConstraintResolver, DefaultInlineConstraintResolver>();
            services.TryAddSingleton<IPorthorRouter, PorthorRouter>();

            services.TryAddSingleton(configuration);

            services.Configure(setupAction);
        }
    }
}
