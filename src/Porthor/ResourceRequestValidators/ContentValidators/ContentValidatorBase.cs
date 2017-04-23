using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators.ContentValidators
{
    /// <summary>
    /// Represents the base class for all content validators.
    /// </summary>
    public abstract class ContentValidatorBase : IResourceRequestValidator
    {
        public ContentValidatorBase(string template)
        { }

        public abstract Task<HttpResponseMessage> ValidateAsync(HttpContext context);
    }
}
