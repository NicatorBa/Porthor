using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Porthor.Models;
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
                        EndpointUrl = $"http://example.org/api/v5.1/data"
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
                        EndpointUrl = $"http://example.org/api/v5.2/data"
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
    }
}
