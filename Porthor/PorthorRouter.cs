using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace Porthor
{
    public class PorthorRouter : IPorthorRouter
    {
        private IRouter _router;

        public PorthorRouter()
        {
            _router = new RouteCollection();
        }

        public IRouter Router
        {
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Router));
                }

                _router = value;
            }
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _router.GetVirtualPath(context);
        }

        public Task RouteAsync(Microsoft.AspNetCore.Routing.RouteContext context)
        {
            return _router.RouteAsync(context);
        }
    }
}
