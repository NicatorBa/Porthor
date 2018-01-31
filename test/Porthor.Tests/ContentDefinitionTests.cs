using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Porthor.Models;
using Porthor.ResourceRequestValidators.ContentValidators;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Porthor.Tests
{
    public class ContentDefinitionTests
    {
        [Fact]
        public async Task Request_WithInvalidMediaType_ReturnsUnsupportedMediaType()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor(options =>
                    {
                        options.Content.ValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Post,
                        Path = "api/v5.1/data",
                        ContentDefinitions = new List<ContentDefinition>
                        {
                            new ContentDefinition{ MediaType = PorthorConstants.MediaType.Json }
                        },
                        EndpointUrl = "http://example.org/api/v5.1/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/v5.1/data");
            requestMessage.Content = new StringContent("Request Body");
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
                        options.Content.ValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Post,
                        Path = "api/v5.2/data",
                        ContentDefinitions = new List<ContentDefinition>
                        {
                            new ContentDefinition{ MediaType = PorthorConstants.MediaType.Json }
                        },
                        EndpointUrl = "http://example.org/api/v5.2/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/v5.2/data");
            requestMessage.Content = new StringContent("{ \"name\": \"demo\" }", Encoding.UTF8, PorthorConstants.MediaType.Json);
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
                    services.AddPorthor(options =>
                    {
                        options.Content.ValidationEnabled = true;
                        options.Content.Add<JsonValidator>(PorthorConstants.MediaType.Json);
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Post,
                        Path = "api/v5.3/data",
                        ContentDefinitions = new List<ContentDefinition>
                        {
                            new ContentDefinition{
                                MediaType = PorthorConstants.MediaType.Json,
                                Template = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\", \"title\": \"Person\", \"type\": \"object\", \"additionalProperties\": false, \"required\": [ \"name\" ], \"properties\": { \"name\": { \"type\": \"string\" } } }"
                            }
                        },
                        EndpointUrl = "http://example.org/api/v5.3/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/v5.3/data");
            requestMessage.Content = new StringContent("{ \"company\": \"demo\" }", Encoding.UTF8, PorthorConstants.MediaType.Json);
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
                        options.Content.ValidationEnabled = true;
                        options.Content.Add<JsonValidator>(PorthorConstants.MediaType.Json);
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Post,
                        Path = "api/v5.4/data",
                        ContentDefinitions = new List<ContentDefinition>
                        {
                            new ContentDefinition{
                                MediaType = PorthorConstants.MediaType.Json,
                                Template = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\", \"title\": \"Person\", \"type\": \"object\", \"additionalProperties\": false, \"required\": [ \"name\" ], \"properties\": { \"name\": { \"type\": \"string\" } } }"
                            }
                        },
                        EndpointUrl = "http://example.org/api/v5.4/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/v5.4/data");
            requestMessage.Content = new StringContent("{ \"name\": \"demo\" }", Encoding.UTF8, PorthorConstants.MediaType.Json);
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }
    }
}
