using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Porthor.Validation
{
    /// <summary>
    /// Validator to validate the current user against policies.
    /// </summary>
    public class AuthorizationValidator : IValidator
    {
        private readonly IEnumerable<string> _policies;

        /// <summary>
        /// Creates a new instance of <see cref="AuthorizationValidator"/>.
        /// </summary>
        /// <param name="policies">Collection of policies.</param>
        public AuthorizationValidator(IEnumerable<string> policies)
        {
            _policies = policies;
        }

        /// <inheritdoc />
        public async Task<ValidationResult> ValidateAsync(HttpContext context)
        {
            var authorizationService = context.RequestServices.GetRequiredService<IAuthorizationService>();

            foreach (var policy in _policies)
            {
                var result = await authorizationService.AuthorizeAsync(context.User, policy);
                if (result.Succeeded)
                {
                    return ValidationResult.Success;
                }
            }

            return ValidationResult.Failed(HttpStatusCode.Forbidden);
        }
    }
}
