using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    /// <summary>
    /// Provides default implementation to create section from the currently active route for an endpoint uri.
    /// </summary>
    public class RouteValueSection : IEndpointUriSection
    {
        private readonly string _key;

        /// <summary>
        /// Creates a new instance of <see cref="RouteValueSection"/>.
        /// </summary>
        /// <param name="key">The key to get a value from <see cref="RouteValueDictionary"/>.</param>
        public RouteValueSection(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Creates a section for the endpoint uri.
        /// </summary>
        /// <param name="values">Route values of current route.</param>
        /// <returns>Current enpoint uri section.</returns>
        public string CreateSection(RouteValueDictionary values)
        {
            return values[_key].ToString();
        }
    }
}
