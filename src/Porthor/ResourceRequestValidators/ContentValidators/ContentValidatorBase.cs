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
        /// <summary>
        /// Constructs a new instance of <see cref="ContentValidatorBase"/>.
        /// </summary>
        /// <param name="template"></param>
        protected ContentValidatorBase(string template)
        { }

        /// <summary>
        /// Validates the content of the current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>
        /// The <see cref="Task{HttpRequestMessage}"/> that represents the asynchronous query string validation process.
        /// Returns null if the content is valid.
        /// </returns>
        public abstract Task<HttpResponseMessage> ValidateAsync(HttpContext context);
    }
}
