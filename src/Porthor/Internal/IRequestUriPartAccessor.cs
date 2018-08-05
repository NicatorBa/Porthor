using Microsoft.AspNetCore.Routing;

namespace Porthor.Internal
{
    /// <summary>
    /// Provides an abstraction of an accessor for an uri part.
    /// </summary>
    public interface IRequestUriPartAccessor
    {
        /// <summary>
        /// Gets the part for the request uri.
        /// </summary>
        /// <param name="routeValues">Route values of current route.</param>
        /// <returns>An uri part.</returns>
        string GetUriPart(RouteValueDictionary routeValues);
    }
}
