using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Porthor.Validation
{
    /// <summary>
    /// Validator to verify authentication of current user.
    /// </summary>
    public class AuthenticationValidator : IValidator
    {
        /// <inheritdoc />
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
