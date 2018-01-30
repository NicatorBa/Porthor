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
    public class AuthorizationTests
    {
        [Fact]
        public async Task Request_WithInvalidPolicy_ReturnsForbidden()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy("InvalidPolicy", policy =>
                        policy.RequireAssertion(context => false));
                    });

                    services.AddPorthor(options =>
                    {
                        options.Security.AuthorizationValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v3.1/data",
                        SecuritySettings = new SecuritySettings
                        {
                            Policies = new List<string>
                            {
                                "InvalidPolicy"
                            }
                        },
                        EndpointUrl = $"http://example.org/api/v3.1/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v3.1/data");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithValidPolicy_ReturnsForbidden()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy("ValidPolicy", policy =>
                        policy.RequireAssertion(context => true));
                    });

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
                        options.Security.AuthorizationValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v3.2/data",
                        SecuritySettings = new SecuritySettings
                        {
                            Policies = new List<string>
                            {
                                "ValidPolicy"
                            }
                        },
                        EndpointUrl = $"http://example.org/api/v3.2/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v3.2/data");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }
    }
}
