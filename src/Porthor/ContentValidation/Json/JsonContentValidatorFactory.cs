using NJsonSchema;
using System;
using System.Threading.Tasks;

namespace Porthor.ContentValidation.Json
{
    public class JsonContentValidatorFactory : ContentValidatorFactory
    {
        public override async Task<IContentValidator> CreateContentValidatorAsync(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            var schema = await JsonSchema4.FromJsonAsync(template);
            return new JsonContentValidator(schema);
        }
    }
}
