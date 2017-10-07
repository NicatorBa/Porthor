using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators
{
    /// <summary>
    /// Request validator for authorization by poliyies.
    /// </summary>
    public class AuthorizationValidator : IResourceRequestValidator
    {
        private readonly IEnumerable<string> _policies;

        /// <summary>
        /// Constructs a new instance of <see cref="AuthorizationValidator"/>.
        /// </summary>
        /// <param name="policies">Collection of policy names.</param>
        public AuthorizationValidator(IEnumerable<string> policies)
        {
            _policies = policies;
        }

        /// <summary>
        /// Validates the current <see cref="HttpContext"/> against policies.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>
        /// The <see cref="Task{HttpRequestMessage}"/> that represents the asynchronous validation process.
        /// Returns null if the current user meets any policy.
        /// </returns>
        public async Task<HttpResponseMessage> ValidateAsync(HttpContext context)
        {
            IAuthorizationService authorizationService = (IAuthorizationService)context.RequestServices.GetService(typeof(IAuthorizationService));
            if (authorizationService == null)
            {
                throw new InvalidOperationException(nameof(IAuthorizationService));
            }

            bool authorized = false;
            foreach (var policy in _policies)
            {
                if (await authorizationService.AuthorizeAsync(context.User, policy))
                {
                    authorized = true;
                    break;
                }
            }
            if (!authorized)
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            return null;
        }
    }
}
