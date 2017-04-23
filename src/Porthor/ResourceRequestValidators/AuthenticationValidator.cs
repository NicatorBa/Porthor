using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators
{
    public class AuthenticationValidator : IResourceRequestValidator
    {
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
