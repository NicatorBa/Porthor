using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Porthor.Models;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Porthor.Tests
{
    public class AuthenticationTests
    {
        [Fact]
        public async Task Request_WithAnonymousUser_ReturnsUnauthorized()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddPorthor(options =>
                    {
                        options.Security.AuthenticationValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v2.1/data",
                        EndpointUrl = "http://example.org/api/v2.1/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v2.1/data");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithAllowAnonymous_ReturnsOk()
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
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        };
                        options.Security.AuthenticationValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v2.2/data",
                        SecuritySettings = new SecuritySettings
                        {
                            AllowAnonymous = true
                        },
                        EndpointUrl = "http://example.org/api/v2.2/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v2.2/data");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithAuthenticatedUser_ReturnsOk()
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
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        };
                        options.Security.AuthenticationValidationEnabled = true;
                    });
                })
                .Configure(app =>
                {
                    app.Use((context, next) =>
                    {
                        ClaimsIdentity identity = new ClaimsIdentity("TestCookieAuthentication");
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                        context.User = principal;

                        return next();
                    });

                    var resource = new Resource
                    {
                        Method = HttpMethod.Get,
                        Path = "api/v2.3/data",
                        EndpointUrl = "http://example.org/api/v2.3/data"
                    };

                    app.UsePorthor(new[] { resource });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/v2.3/data");
            var responseMessage = await server.CreateClient().SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
        }
    }
}
