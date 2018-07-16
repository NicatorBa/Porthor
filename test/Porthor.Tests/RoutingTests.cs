using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Porthor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Porthor.Tests
{
    public class RoutingTests
    {
        [Fact]
        public async Task Request_WhenSent_ReturnsResponseHeaders()
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
                                Assert.Equal("http://example.org/api/values", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                response.Headers.Add("testHeader", "testHeaderValue");
                                return response;
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

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            responseMessage.Headers.TryGetValues("testHeader", out IEnumerable<string> testHeaderValue);
            Assert.Equal("testHeaderValue", testHeaderValue.Single());
        }

        [Fact]
        public async Task Request_WhenSent_ReturnsResponseContent()
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
                                Assert.Equal("http://example.org/api/values", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new StringContent("Response Body")
                                };
                                return response;
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

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            var responseContent = responseMessage.Content.ReadAsStringAsync();
            Assert.True(responseContent.Wait(3000) && !responseContent.IsFaulted);
            Assert.Equal("Response Body", responseContent.Result);
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        public async Task Request_WithoutBody_ReturnsOk(string method)
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
                                Assert.Equal("http://example.org/api/values", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = new HttpMethod(method),
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values"
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(new HttpMethod(method), "api/values");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Theory]
        [InlineData("POST")]
        [InlineData("PUT")]
        public async Task Request_WithBody_ReturnsOk(string method)
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
                                Assert.Equal("http://example.org/api/values", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                var content = request.Content.ReadAsStringAsync();
                                Assert.True(content.Wait(3000) && !content.IsFaulted);
                                Assert.Equal("Request Body", content.Result);
                                return response;
                            }
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = new HttpMethod(method),
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values"
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(new HttpMethod(method), "api/values")
            {
                Content = new StringContent("Request Body")
            };
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("DELETE")]
        public async Task Request_WithoutPassedBody_ReturnsOk(string method)
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
                                Assert.Equal("http://example.org/api/values", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                Assert.Null(request.Content);
                                return response;
                            }
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = new HttpMethod(method),
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values"
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(new HttpMethod(method), "api/values")
            {
                Content = new StringContent("Request Body")
            };
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithRouteValue_ReturnsOk()
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
                                Assert.Equal("http://example.org/api/values/10", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values/{id}",
                        BackendUrl = "http://example.org/api/values/{id}"
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values/10");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithEnvironmentVariable_ReturnsOk()
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
                                Assert.Equal("http://example.org/api/values", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values",
                        BackendUrl = "http://[DOMAIN]/api/values"
                    };

                    app.UsePorthor(new[] { routingRule });
                })
                .ConfigureAppConfiguration(config =>
                {
                    var defaults = new Dictionary<string, string>
                    {
                        { "DOMAIN", "example.org" }
                    };

                    config.AddInMemoryCollection(defaults);
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }
    }
}
