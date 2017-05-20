using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    public class EndpointUriSection : IEndpointUriSection
    {
        private readonly string _value;

        public EndpointUriSection(string value)
        {
            _value = value;
        }

        public string CreateSection(RouteValueDictionary values)
        {
            return _value;
        }
    }
}
