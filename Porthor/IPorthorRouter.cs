using Microsoft.AspNetCore.Routing;

namespace Porthor
{
    public interface IPorthorRouter : IRouter
    {
        IRouter Router { set; }
    }
}
