using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Porthor.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Porthor.Tests
{
    public class TimeoutTests
    {
        [Fact]
        public async Task Request_WaitForDefaultTimeout_ReturnsGatewayTimeout()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor()
                        .AddMessageHandler(new TestMessageHandler
                        {
                            Sender = (request, cancellationToken) =>
                            {
                                while (!cancellationToken.IsCancellationRequested) { }
                                cancellationToken.ThrowIfCancellationRequested();

                                return new HttpResponseMessage();
                            }
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values"
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            // Act
            var sw = Stopwatch.StartNew();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values");
            var responseMessage = await client.SendAsync(requestMessage);
            sw.Stop();

            // Assert
            Assert.Equal(HttpStatusCode.GatewayTimeout, responseMessage.StatusCode);
            Assert.Equal(100, sw.Elapsed.TotalSeconds, 0);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(120)]
        public async Task Request_WaitForTimeout_ReturnsGatewayTimeout(int timeout)
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor()
                        .AddMessageHandler(new TestMessageHandler
                        {
                            Sender = (request, cancellationToken) =>
                            {
                                while (!cancellationToken.IsCancellationRequested) { }
                                cancellationToken.ThrowIfCancellationRequested();

                                return new HttpResponseMessage();
                            }
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values",
                        Timeout = timeout
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);
            var client = server.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            // Act
            var sw = Stopwatch.StartNew();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values");
            var responseMessage = await client.SendAsync(requestMessage);
            sw.Stop();

            // Assert
            Assert.Equal(HttpStatusCode.GatewayTimeout, responseMessage.StatusCode);
            Assert.Equal(timeout, sw.Elapsed.TotalSeconds, 0);
        }
    }
}
