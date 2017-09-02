using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    /// <summary>
    /// Provides default implementation to get static section for an endpoint uri.
    /// </summary>
    public class EndpointUriSection : IEndpointUriSection
    {
        private readonly string _value;

        /// <summary>
        /// Creates a new instance of <see cref="EndpointUriSection"/>.
        /// </summary>
        /// <param name="value">The value for this section.</param>
        public EndpointUriSection(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a section for the endpoint uri.
        /// </summary>
        /// <param name="values">Route values of current route.</param>
        /// <returns>Current enpoint uri section.</returns>
        public string CreateSection(RouteValueDictionary values)
        {
            return _value;
        }
    }
}
