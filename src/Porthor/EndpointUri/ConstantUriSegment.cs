using Microsoft.AspNetCore.Routing;

namespace Porthor.EndpointUri
{
    public class ConstantUriSegment : IEndpointUriSegment
    {
        private readonly string _constant;

        public ConstantUriSegment(string constant)
        {
            _constant = constant;
        }

        public string GetSegment(RouteValueDictionary values)
        {
            return _constant;
        }
    }
}
