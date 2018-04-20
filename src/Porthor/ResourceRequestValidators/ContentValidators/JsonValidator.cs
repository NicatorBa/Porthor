using Microsoft.AspNetCore.Http;
using NJsonSchema;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators.ContentValidators
{
    /// <summary>
    /// Request validator for json content.
    /// </summary>
    public class JsonValidator : ContentValidatorBase
    {
        private readonly JsonSchema4 _schema;

        /// <summary>
        /// Constructs a new instance of <see cref="JsonValidator"/>.
        /// </summary>
        /// <param name="template">Template for json schema.</param>
        public JsonValidator(string template) : base(template)
        {
            _schema = JsonSchema4.FromJsonAsync(template).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Validates the content of the current <see cref="HttpContext"/> agains the json schema.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>
        /// The <see cref="Task{HttpRequestMessage}"/> that represents the asynchronous query string validation process.
        /// Returns null if the content is valid agains the json schema.
        /// </returns>
        public override async Task<HttpResponseMessage> ValidateAsync(HttpContext context)
        {
            var errors = _schema.Validate(await StreamToString(context.Request.Body));
            if (errors.Any())
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return null;
        }

        private Task<string> StreamToString(Stream stream)
        {
            var streamContent = new StreamContent(stream);
            stream.Position = 0;
            return streamContent.ReadAsStringAsync();
        }
    }
}
