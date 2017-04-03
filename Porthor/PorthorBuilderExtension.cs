using System;

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
    }
}
