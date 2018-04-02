using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
    public class ProxyTests
    {
        [Theory]
        [InlineData("GET", "api/v1.1/data")]
        [InlineData("POST", "api/v1.2/data")]
        [InlineData("PUT", "api/v1.3/data")]
        [InlineData("DELETE", "api/v1.4/data")]
        public async Task Request_WithoutBody_ReturnsResponseHeader(string method, string path)
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
                                Assert.Equal($"http://example.org/{path}", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                response.Headers.Add("testHeader", "testHeaderValue");
                                response.Content = new StringContent("Response Body");
                                return response;
                            }
                        };
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = new HttpMethod(method),
                        Path = path,
                        EndpointUrl = $"http://example.org/{path}"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(new HttpMethod(method), path);
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            var responseContent = responseMessage.Content.ReadAsStringAsync();
            Assert.True(responseContent.Wait(3000) && !responseContent.IsFaulted);
            Assert.Equal("Response Body", responseContent.Result);
            responseMessage.Headers.TryGetValues("testHeader", out IEnumerable<string> testHeaderValue);
            Assert.Equal("testHeaderValue", testHeaderValue.Single());
        }

        [Theory]
        [InlineData("POST", "api/v1.5/data")]
        [InlineData("PUT", "api/v1.6/data")]
        public async Task Request_WithBody_ReturnsResponse(string method, string path)
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
                                Assert.Equal($"http://example.org/{path}", request.RequestUri.ToString());
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                var content = request.Content.ReadAsStringAsync();
                                Assert.True(content.Wait(3000) && !content.IsFaulted);
                                Assert.Equal("Request Body", content.Result);
                                response.Content = new StringContent("Response Body");
                                return response;
                            }
                        };
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = new HttpMethod(method),
                        Path = path,
                        EndpointUrl = $"http://example.org/{path}"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(new HttpMethod(method), path)
            {
                Content = new StringContent("Request Body")
            };
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            var responseContent = responseMessage.Content.ReadAsStringAsync();
            Assert.True(responseContent.Wait(3000) && !responseContent.IsFaulted);
            Assert.Equal("Response Body", responseContent.Result);
        }
    }
}
