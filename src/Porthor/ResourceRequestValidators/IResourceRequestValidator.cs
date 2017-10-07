using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators
{
    /// <summary>
    /// Provides an abstraction for a request validator.
    /// </summary>
    public interface IResourceRequestValidator
    {
        /// <summary>
        /// Validates the current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>
        /// The <see cref="Task{HttpRequestMessage}"/> that represents the asynchronous validation process.
        /// Must be null if validation was successful.
        /// </returns>
        Task<HttpResponseMessage> ValidateAsync(HttpContext context);
    }
}
