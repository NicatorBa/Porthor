using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    /// <summary>
    /// Provides an abstraction of an endpoint uri section.
    /// </summary>
    public interface IEndpointUriSection
    {
        /// <summary>
        /// Creates a section for the endpoint uri.
        /// </summary>
        /// <param name="values">Route values of current route.</param>
        /// <returns>Current enpoint uri section.</returns>
        string CreateSection(RouteValueDictionary values);
    }
}
