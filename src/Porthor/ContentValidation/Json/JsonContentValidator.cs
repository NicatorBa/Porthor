using Microsoft.AspNetCore.Http;
using NJsonSchema;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Porthor.ContentValidation.Json
{
    public class JsonContentValidator : IContentValidator
    {
        private readonly JsonSchema4 _schema;
        public JsonContentValidator(JsonSchema4 schema)
        {
            _schema = schema;
        }

        public async Task<bool> Validate(HttpRequest request)
        {
            var errors = _schema.Validate(await StreamToString(request.Body));
            if (errors.Count > 0)
            {
                return false;
            }

            return true;
        }

        private Task<string> StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEndAsync();
            }
        }
    }
}
