using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Porthor.Configuration;

namespace Porthor.Validation
{
    /// <summary>
    /// Validator for request content by media type.
    /// </summary>
    public class MediaTypeContentValidator : IValidator
    {
        private readonly IDictionary<string, ContentValidator> _validators = new Dictionary<string, ContentValidator>();

        /// <summary>
        /// Creates a new instance of <see cref="MediaTypeContentValidator"/>.
        /// </summary>
        /// <param name="options">Content options.</param>
        /// <param name="contents">Settings for content.</param>
        public MediaTypeContentValidator(
            ContentOptions options,
            IEnumerable<Models.Content> contents)
        {
            foreach (var content in contents)
            {
                if (string.IsNullOrEmpty(content.Schema))
                {
                    _validators.Add(content.MediaType, null);
                }
                else
                {
                    var validator = options.CreateContentValidator(content.MediaType, content.Schema);
                    _validators.Add(content.MediaType, validator);
                }
            }
        }

        /// <inheritdoc />
        public async Task<ValidationResult> ValidateAsync(HttpContext context)
        {
            var contentType = new ContentType(context.Request.ContentType);
            var mediaTypeValidatorPair = _validators.SingleOrDefault(kvp => contentType.MediaType.StartsWith(kvp.Key));
            if (mediaTypeValidatorPair.Key == null)
            {
                return ValidationResult.Failed(HttpStatusCode.UnsupportedMediaType);
            }

            var validator = mediaTypeValidatorPair.Value;
            if (validator == null)
            {
                return ValidationResult.Success;
            }

            context.Request.EnableRewind();

            return await validator.ValidateAsync(context);
        }
    }
}
