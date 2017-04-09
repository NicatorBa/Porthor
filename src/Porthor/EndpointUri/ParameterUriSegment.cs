using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    public class ParameterUriSegment : IEndpointUriSegment
    {
        private readonly string _valueKey;

        public ParameterUriSegment(string valueKey)
        {
            _valueKey = valueKey;
        }

        public string GetSegment(RouteValueDictionary values)
        {
            return values[_valueKey].ToString();
        }
    }
}
