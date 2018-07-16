using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Porthor.Constants;
using Porthor.Models;
using Porthor.Validation.Content;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Porthor.Tests
{
    public class ContentTests
    {
        [Fact]
        public async Task Request_WithInvalidMediaType_ReturnsUnsupportedMediaType()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor()
                        .ConfigureContentValidation(options => options.Enabled = true);
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values",
                        Validations = new Validations
                        {
                            Contents = new Contents
                            {
                                new Content{ MediaType = MediaType.Application.Json }
                            }
                        }
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values")
            {
                Content = new StringContent("Request body")
            };
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithValidMediaType_ReturnsOk()
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
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        })
                        .ConfigureContentValidation(options => options.Enabled = true);
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values",
                        Validations = new Validations
                        {
                            Contents = new Contents
                            {
                                new Content{ MediaType = MediaType.Application.Json }
                            }
                        }
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values")
            {
                Content = new StringContent("{ \"name\": \"demo\" }", Encoding.UTF8, MediaType.Application.Json)
            };
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithInvalidJsonContent_ReturnsBadRequest()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor()
                        .ConfigureContentValidation(options =>
                        {
                            options.Enabled = true;
                            options.Add<JsonValidator>(MediaType.Application.Json);
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values",
                        Validations = new Validations
                        {
                            Contents = new Contents
                            {
                                new Content
                                {
                                    MediaType = MediaType.Application.Json,
                                    Schema = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\", \"title\": \"Person\", \"type\": \"object\", \"additionalProperties\": false, \"required\": [ \"name\" ], \"properties\": { \"name\": { \"type\": \"string\" } } }"
                                }
                            }
                        }
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values")
            {
                Content = new StringContent("{ \"company\": \"demo\" }", Encoding.UTF8, MediaType.Application.Json)
            };
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithValidJsonContent_ReturnsOk()
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
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        })
                        .ConfigureContentValidation(options =>
                        {
                            options.Enabled = true;
                            options.Add<JsonValidator>(MediaType.Application.Json);
                        });
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values",
                        Validations = new Validations
                        {
                            Contents = new Contents
                            {
                                new Content
                                {
                                    MediaType = MediaType.Application.Json,
                                    Schema = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\", \"title\": \"Person\", \"type\": \"object\", \"additionalProperties\": false, \"required\": [ \"name\" ], \"properties\": { \"name\": { \"type\": \"string\" } } }"
                                }
                            }
                        }
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values")
            {
                Content = new StringContent("{ \"name\": \"demo\" }", Encoding.UTF8, MediaType.Application.Json)
            };
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }
    }
}
