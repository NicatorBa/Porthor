using Microsoft.AspNetCore.Routing;

namespace Porthor.Internal
{
    /// <summary>
    /// Implementation to get the dynamic uri part based on route values.
    /// </summary>
    public class RouteValueUriPartAccessor : IRequestUriPartAccessor
    {
        private readonly string _key;

        /// <summary>
        /// Creates a new instance of <see cref="RouteValueUriPartAccessor"/>.
        /// </summary>
        /// <param name="key">The key of the uri part in <see cref="RouteValueDictionary"/>.</param>
        public RouteValueUriPartAccessor(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Gets the part for the request uri.
        /// </summary>
        /// <param name="routeValues">Route values of current route.</param>
        /// <returns>An uri part.</returns>
        public string GetUriPart(RouteValueDictionary routeValues)
        {
            return routeValues[_key].ToString();
        }
    }
}
