using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Porthor.Tests
{
    internal class TestMessageHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> Sender { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Sender != null)
            {
                return Task.FromResult(Sender(request, cancellationToken));
            }

            return Task.FromResult<HttpResponseMessage>(null);
        }
    }
}
