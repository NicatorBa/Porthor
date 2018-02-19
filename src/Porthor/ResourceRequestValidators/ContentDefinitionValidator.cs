using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Porthor.Models;
using Porthor.ResourceRequestValidators.ContentValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators
{
    /// <summary>
    /// Request validator for content.
    /// </summary>
    public class ContentDefinitionValidator : IResourceRequestValidator
    {
        private readonly IDictionary<string, ContentValidatorBase> _validators = new Dictionary<string, ContentValidatorBase>();

        /// <summary>
        /// Constructs a new instance of <see cref="ContentDefinitionValidator"/>.
        /// </summary>
        /// <param name="contentDefinitions">Collection of <see cref="ContentDefinition"/>.</param>
        /// <param name="options">Content configuration options.</param>
        public ContentDefinitionValidator(
            IEnumerable<ContentDefinition> contentDefinitions,
            ContentOptions options)
        {
            foreach (var definition in contentDefinitions)
            {
                if (string.IsNullOrEmpty(definition.Template))
                {
                    _validators.Add(definition.MediaType, null);
                }
                else
                {
                    var validator = options.Get(definition.MediaType, definition.Template);
                    if (validator == null)
                    {
                        throw new NotSupportedException(definition.MediaType);
                    }
                    _validators.Add(definition.MediaType, validator);
                }
            }
        }

        /// <summary>
        /// Validates the content of the current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>
        /// The <see cref="Task{HttpRequestMessage}"/> that represents the asynchronous query string validation process.
        /// Returns null if the content is valid.
        /// </returns>
        public async Task<HttpResponseMessage> ValidateAsync(HttpContext context)
        {
            var mediaTypeValidator = _validators.SingleOrDefault(v => context.Request.ContentType.Contains(v.Key));
            if (mediaTypeValidator.Key == null)
            {
                return new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }

            var validator = mediaTypeValidator.Value;
            if (validator == null)
            {
                return null;
            }

            context.Request.EnableRewind();
            return await validator.ValidateAsync(context);
        }
    }
}
