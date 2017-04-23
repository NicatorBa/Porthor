using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators
{
    public class AuthorizationValidator : IResourceRequestValidator
    {
        private readonly IEnumerable<string> _policies;

        public AuthorizationValidator(IEnumerable<string> policies)
        {
            _policies = policies;
        }

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
