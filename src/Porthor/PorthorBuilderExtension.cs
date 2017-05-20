using Porthor;
using Porthor.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extension method for adding the <see cref="PorthorMiddleware"/> to an <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class PorthorBuilderExtension
    {
        /// <summary>
        /// Adds a <see cref="PorthorMiddleware"/> to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UsePorthor(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            return builder.UseMiddleware<PorthorMiddleware>();
        }

        /// <summary>
        /// Adds a <see cref="PorthorMiddleware"/> to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="resources">A collection of resources to initialize startup routes.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UsePorthor(this IApplicationBuilder builder, IEnumerable<Resource> resources)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }
            var router = (IPorthorRouter)builder.ApplicationServices.GetService(typeof(IPorthorRouter));
            router.Initialize(resources).Wait();
            return builder.UseMiddleware<PorthorMiddleware>();
        }
    }
}
