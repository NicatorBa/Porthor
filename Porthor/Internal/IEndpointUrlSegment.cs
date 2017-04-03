using System.Collections.Generic;

namespace Porthor.Internal
{
    internal interface IEndpointUrlSegment
    {
        string GetSegment(IDictionary<string, string> parameters);
    }
}
