using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Porthor.Validation
{
    /// <summary>
    /// Provides an abstraction for a validator.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates the current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>The <see cref="Task{ValidationResult}"/> that represents the asynchronous validation process.</returns>
        Task<ValidationResult> ValidateAsync(HttpContext context);
    }
}
