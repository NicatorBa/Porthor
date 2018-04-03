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
                    services.AddPorthor(options =>
                    {
                        options.BackChannelMessageHandler = new TestMessageHandler
                        {
                            Sender = (request, cancellationToken) =>
                            {
                                while (true)
                                {
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                            }
                        };
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v7.1/data",
                        EndpointUrl = "http://example.org/api/v7.1/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v7.1/data");
            var httpClient = server.CreateClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            var responseMessage = await httpClient.SendAsync(requestMessage);
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
                    services.AddPorthor(options =>
                    {
                        options.BackChannelMessageHandler = new TestMessageHandler
                        {
                            Sender = (request, cancellationToken) =>
                            {
                                while (true)
                                {
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                            }
                        };
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v7.2/data",
                        EndpointUrl = "http://example.org/api/v7.1/data",
                        Timeout = timeout
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            Stopwatch sw = Stopwatch.StartNew();
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v7.2/data");
            var httpClient = server.CreateClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            var responseMessage = await httpClient.SendAsync(requestMessage);
            sw.Stop();

            // Assert
            Assert.Equal(HttpStatusCode.GatewayTimeout, responseMessage.StatusCode);
            Assert.Equal(timeout, sw.Elapsed.TotalSeconds, 0);
        }
    }
}
