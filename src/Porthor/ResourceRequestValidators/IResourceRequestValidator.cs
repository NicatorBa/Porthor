using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators
{
    public interface IResourceRequestValidator
    {
        Task<HttpResponseMessage> ValidateAsync(HttpContext context);
    }
}
