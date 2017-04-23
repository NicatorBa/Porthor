using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    public interface IEndpointUriSection
    {
        string CreateSection(RouteValueDictionary values);
    }
}
