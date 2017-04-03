using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porthor
{
    public class ResourceConfigurator : IResourceConfigurator
    {
        private readonly ICollection<Resource> _resources = new List<Resource>();

        private readonly IPorthorRouter _router;
        private readonly IInlineConstraintResolver _inlineConstraintResolver;

        public ResourceConfigurator(
            IPorthorRouter router,
            IInlineConstraintResolver resolver)
        {
            _router = router;
            _inlineConstraintResolver = resolver;
        }

        public ICollection<Resource> Resources { get { return _resources; } }

        public Task InitializeAsync()
        {
            var routeCollection = new RouteCollection();
            foreach (var resource in _resources)
            {
                routeCollection.Add(new RouteBuilder(resource).Build(_inlineConstraintResolver));
            }
            _router.Router = routeCollection;
            return Task.CompletedTask;
        }
    }
}
