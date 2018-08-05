using Microsoft.AspNetCore.Http;
using NJsonSchema;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.Validation.Content
{
    /// <summary>
    /// Validator for json content.
    /// </summary>
    public class JsonValidator : ContentValidator
    {
        private readonly JsonSchema4 _jsonSchema;

        /// <summary>
        /// Creates a new instance of <see cref="JsonValidator"/>.
        /// </summary>
        /// <param name="schema">Json validation schema.</param>
        public JsonValidator(string schema) : base(schema)
        {
            _jsonSchema = JsonSchema4.FromJsonAsync(Schema).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Validates the json of current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>The <see cref="Task{ValidationResult}"/> that represents the asynchronous validation process.</returns>
        public override async Task<ValidationResult> ValidateAsync(HttpContext context)
        {
            var errors = _jsonSchema.Validate(await StreamToString(context.Request.Body));
            if (errors.Any())
            {
                return ValidationResult.Failed();
            }

            return ValidationResult.Success;
        }

        private Task<string> StreamToString(Stream stream)
        {
            var content = new StreamContent(stream);
            stream.Position = 0;

            return content.ReadAsStringAsync();
        }
    }
}
