using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace Porthor.Validation
{
    /// <summary>
    /// Validator to verify authentication of current user.
    /// </summary>
    public class AuthenticationValidator : IValidator
    {
        /// <summary>
        /// Validates that the user of the current <see cref="HttpContext"/> is authenticated.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>The <see cref="Task{ValidationResult}"/> that represents the asynchronous validation process.</returns>
        public Task<ValidationResult> ValidateAsync(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return Task.FromResult(ValidationResult.Failed(HttpStatusCode.Unauthorized));
            }

            return Task.FromResult(ValidationResult.Success);
        }
    }
}
