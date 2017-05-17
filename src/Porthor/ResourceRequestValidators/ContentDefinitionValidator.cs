using Microsoft.AspNetCore.Http;
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
    public class ContentDefinitionValidator : IResourceRequestValidator
    {
        private readonly IDictionary<string, ContentValidatorBase> _validators = new Dictionary<string, ContentValidatorBase>();

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

        public async Task<HttpResponseMessage> ValidateAsync(HttpContext context)
        {
            try
            {
                var validator = _validators.Single(v => context.Request.ContentType.Contains(v.Key)).Value;
                if (validator == null)
                {
                    return null;
                }

                return await validator.ValidateAsync(context);
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }
        }
    }
}
