using Microsoft.Extensions.DependencyInjection;
using Porthor;
using Porthor.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension methods for adding the <see cref="PorthorMiddleware"/> to an <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class PorthorBuilderExtensions
    {
        /// <summary>
        /// Adds a <see cref="PorthorMiddleware"/> to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="routingRules">Collection of routing rules to initialize startup routes.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UsePorthor(this IApplicationBuilder app, IEnumerable<RoutingRule> routingRules = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var router = app.ApplicationServices.GetService<IPorthorRouter>();
            if (router == null)
            {
                throw new InvalidOperationException(nameof(IPorthorRouter));
            }

            if (routingRules != null)
            {
                router.InitializeAsync(routingRules).Wait();
            }

            return app.UseMiddleware<PorthorMiddleware>();
        }
    }
}
