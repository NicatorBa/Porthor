using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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

        /// <inheritdoc />
        public abstract Task<ValidationResult> ValidateAsync(HttpContext context);
    }
}
