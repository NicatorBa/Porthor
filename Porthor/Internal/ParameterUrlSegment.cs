using System.Collections.Generic;

namespace Porthor.Internal
{
    internal class ParameterUrlSegment : IEndpointUrlSegment
    {
        private readonly string _parameterKey;

        public ParameterUrlSegment(string parameterKey)
        {
            _parameterKey = parameterKey;
        }

        public string GetSegment(IDictionary<string, string> parameters)
        {
            return parameters[_parameterKey];
        }
    }
}
