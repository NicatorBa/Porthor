using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porthor
{
    public interface IResourceConfigurator
    {
        ICollection<Resource> Resources { get; }

        /// <summary>
        /// Initialize the API resources for gateway routing.
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();
    }
}
