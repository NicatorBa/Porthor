using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Porthor.Validation
{
    /// <summary>
    /// Provides an abstraction for a content validator.
    /// </summary>
    public abstract class ContentValidator : IValidator
    {
        /// <summary>
        /// Creates a new instance of <see cref="ContentValidator"/>.
        /// </summary>
        /// <param name="schema">Content validation schema.</param>
        protected ContentValidator(string schema)
        {
            Schema = schema;
        }

        /// <summary>
        /// Validation schema for content.
        /// </summary>
        protected string Schema { get; }

        /// <summary>
        /// Validates the content of current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>The <see cref="Task{ValidationResult}"/> that represents the asynchronous validation process.</returns>
        public abstract Task<ValidationResult> ValidateAsync(HttpContext context);
    }
}
