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
        [InlineData("GET", "api/v1/data")]
        [InlineData("POST", "api/v2/data")]
        [InlineData("PUT", "api/v3/data")]
        [InlineData("DELETE", "api/v4/data")]
        public async Task Request_WithoutBody_ReturnsResponseHeader(string method, string path)
        {
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
                        Name = "Test",
                        Method = new HttpMethod(method),
                        Path = path,
                        EndpointUrl = $"http://example.org/{path}"
                    };

                    app.UsePorthor(new[] { resource });
                });

            var server = new TestServer(builder);

            var requestMessage = new HttpRequestMessage(new HttpMethod(method), path);
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            var responseContent = responseMessage.Content.ReadAsStringAsync();
            Assert.True(responseContent.Wait(3000) && !responseContent.IsFaulted);
            Assert.Equal("Response Body", responseContent.Result);
            IEnumerable<string> testHeaderValue;
            responseMessage.Headers.TryGetValues("testHeader", out testHeaderValue);
            Assert.Equal("testHeaderValue", testHeaderValue.Single());
        }

        [Theory]
        [InlineData("POST", "api/v5/data")]
        [InlineData("PUT", "api/v6/data")]
        public async Task Request_WithBody_ReturnsResponse(string method, string path)
        {
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
                        Name = "Test",
                        Method = new HttpMethod(method),
                        Path = path,
                        EndpointUrl = $"http://example.org/{path}"
                    };

                    app.UsePorthor(new[] { resource });
                });

            var server = new TestServer(builder);

            var requestMessage = new HttpRequestMessage(new HttpMethod(method), path);
            requestMessage.Content = new StringContent("Request Body");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            var responseContent = responseMessage.Content.ReadAsStringAsync();
            Assert.True(responseContent.Wait(3000) && !responseContent.IsFaulted);
            Assert.Equal("Response Body", responseContent.Result);
        }
    }
}
