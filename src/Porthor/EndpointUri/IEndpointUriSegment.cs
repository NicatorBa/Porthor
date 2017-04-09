using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    public interface IEndpointUriSegment
    {
        string GetSegment(RouteValueDictionary values);
    }
}
