using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Porthor.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Porthor.Tests
{
    public class RouteTests
    {
        [Fact]
        public async Task Request_WithRouteValue_ReturnsOk()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor(options =>
                    {
                        options.BackChannelMessageHandler = new TestMessageHandler
                        {
                            Sender = request =>
                            {
                                Assert.Equal("http://example.org/api/v6.1/samples/10", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        };
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v6.1/data/{id}",
                        EndpointUrl = "http://example.org/api/v6.1/samples/{id}"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v6.1/data/10");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithEnvironmentVariable_ReturnsOk()
        {
            // Arrange
            IConfiguration appConfig = null;
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor(options =>
                    {
                        options.BackChannelMessageHandler = new TestMessageHandler
                        {
                            Sender = request =>
                            {
                                Assert.Equal("http://example.org/api/v6.2/data", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        };
                        options.Configuration = appConfig;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v6.2/data",
                        EndpointUrl = "http://[DOMAIN]/api/v6.2/data"
                    };

                    app.UsePorthor(new[] { resource });
                })
                .ConfigureAppConfiguration(config =>
                {
                    var defaults = new Dictionary<string, string>
                    {
                        {"DOMAIN", "example.org"}
                    };
                    config.AddInMemoryCollection(defaults);

                    appConfig = config.Build();
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v6.2/data");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }
    }
}
