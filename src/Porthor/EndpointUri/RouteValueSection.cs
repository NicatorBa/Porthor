using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    public class RouteValueSection : IEndpointUriSection
    {
        private readonly string _key;

        public RouteValueSection(string key)
        {
            _key = key;
        }

        public string CreateSection(RouteValueDictionary values)
        {
            return values[_key].ToString();
        }
    }
}
