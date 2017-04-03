using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Porthor.Internal
{
    internal interface IHttpMethodStrategy
    {
        Task HandleRouteAsync(HttpContext context);
    }
}
