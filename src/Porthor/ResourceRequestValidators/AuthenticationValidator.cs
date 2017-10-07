using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators
{
    /// <summary>
    /// Request validator to verify authentication level of current user.
    /// </summary>
    public class AuthenticationValidator : IResourceRequestValidator
    {
        /// <summary>
        /// Validates if the user of the current <see cref="HttpContext"/> is authenticated.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>
        /// The <see cref="Task{HttpRequestMessage}"/> that represents the asynchronous validation process.
        /// Returns null if user is authenticated.
        /// </returns>
        public Task<HttpResponseMessage> ValidateAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return Task.FromResult(responseMessage);
            }

            return Task.FromResult<HttpResponseMessage>(null);
        }
    }
}
