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

        /// <inheritdoc />
        public string GetUriPart(RouteValueDictionary routeValues)
        {
            return routeValues[_key].ToString();
        }
    }
}
