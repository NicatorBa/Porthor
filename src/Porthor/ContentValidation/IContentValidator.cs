using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Porthor.ContentValidation
{
    public interface IContentValidator
    {
        Task<bool> Validate(HttpRequest request);
    }
}
