using Microsoft.AspNetCore.Http;
using NJsonSchema;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators.ContentValidators
{
    public class JsonValidator : ContentValidatorBase
    {
        private readonly JsonSchema4 _schema;

        public JsonValidator(string template) : base(template)
        {
            _schema = JsonSchema4.FromJsonAsync(template).GetAwaiter().GetResult();
        }

        public override async Task<HttpResponseMessage> ValidateAsync(HttpContext context)
        {
            var errors = _schema.Validate(await StreamToString(context.Request.Body));
            if (errors.Count > 0)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return null;
        }

        private Task<string> StreamToString(Stream stream)
        {
            stream.Position = 0;
            var streamContent = new StreamContent(stream);
            return streamContent.ReadAsStringAsync();
        }
    }
}
