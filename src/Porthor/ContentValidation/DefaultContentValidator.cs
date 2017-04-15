using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Porthor.ContentValidation
{
    public class DefaultContentValidator : IContentValidator
    {
        public Task<bool> Validate(HttpRequest request)
        {
            return Task.FromResult(true);
        }
    }
}
