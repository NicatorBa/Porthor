using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Porthor.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Porthor.Tests
{
    public class QueryParameterTests
    {
        [Fact]
        public async Task Request_WithoutRequiredQueryParameter_ReturnsBadRequest()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor(options =>
                    {
                        options.QueryStringValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v4.1/data",
                        QueryParameterSettings = new QueryParameterSettings
                        {
                            QueryParameters = new List<QueryParameter>
                            {
                                new QueryParameter
                                {
                                    Name = "query",
                                    Required = true
                                }
                            }
                        },
                        EndpointUrl = "http://example.org/api/v4.1/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v4.1/data");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithRequiredQueryParameter_ReturnsOk()
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
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        };
                        options.QueryStringValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v4.2/data",
                        QueryParameterSettings = new QueryParameterSettings
                        {
                            QueryParameters = new List<QueryParameter>
                            {
                                new QueryParameter
                                {
                                    Name = "query",
                                    Required = true
                                }
                            }
                        },
                        EndpointUrl = "http://example.org/api/v4.2/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v4.2/data?query=test");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithForbiddenQueryParameter_ReturnsBadRequest()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor(options =>
                    {
                        options.QueryStringValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v4.3/data",
                        EndpointUrl = "http://example.org/api/v4.3/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v4.3/data?query=test");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithQueryParameter_ReturnsOk()
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
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        };
                        options.QueryStringValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v4.4/data",
                        QueryParameterSettings = new QueryParameterSettings
                        {
                            QueryParameters = new List<QueryParameter>
                            {
                                new QueryParameter
                                {
                                    Name = "query"
                                }
                            }
                        },
                        EndpointUrl = "http://example.org/api/v4.4/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v4.4/data?query=test");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithAdditionalQueryParameter_ReturnsOk()
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
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        };
                        options.QueryStringValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v4.5/data",
                        QueryParameterSettings = new QueryParameterSettings
                        {
                            AdditionalQueryParameters = true
                        },
                        EndpointUrl = "http://example.org/api/v4.5/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v4.5/data?query=test");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }
    }
}
