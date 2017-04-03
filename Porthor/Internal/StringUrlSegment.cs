using System.Collections.Generic;

namespace Porthor.Internal
{
    internal class StringUrlSegment : IEndpointUrlSegment
    {
        private readonly string _segment;

        public StringUrlSegment(string segment)
        {
            _segment = segment;
        }

        public string GetSegment(IDictionary<string, string> parameters)
        {
            return _segment;
        }
    }
}
