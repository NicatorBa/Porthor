using Microsoft.AspNetCore.Routing;
using Porthor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porthor
{
    /// <summary>
    /// Provides an abstraction for a router.
    /// </summary>
    public interface IPorthorRouter : IRouter
    {
        /// <summary>
        /// Initialize the router with the specified resources.
        /// </summary>
        /// <param name="resources">Collection of API resources.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous initialization process.</returns>
        Task Initialize(IEnumerable<Resource> resources);
    }
}
