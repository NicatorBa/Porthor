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
            var validator = _validators.SingleOrDefault(v => v.Key.Equals(context.Request.ContentType)).Value;
            if (validator == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return await validator.ValidateAsync(context);
        }
    }
}
