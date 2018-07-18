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
                    services.AddPorthor()
                        .EnableAuthenticationValidation();
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
            Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithAllowAnonymous_ReturnsOk()
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
                        .EnableAuthenticationValidation();
                })
                .Configure(app =>
                {
                    var routingRule = new RoutingRule
                    {
                        HttpMethod = HttpMethod.Get,
                        FrontendPath = "api/values",
                        BackendUrl = "http://example.org/api/values",
                        ValidationSettings = new ValidationSettings
                        {
                            Authentication = new Authentication
                            {
                                AllowAnonymous = true
                            }
                        }
                    };

                    app.UsePorthor(new[] { routingRule });
                });
            var server = new TestServer(builder);

            // Act
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/values");
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
                    services.AddPorthor()
                        .AddMessageHandler(new TestMessageHandler
                        {
                            Sender = (request, cancellationToken) =>
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        })
                        .EnableAuthenticationValidation();
                })
                .Configure(app =>
                {
                    app.Use((context, next) =>
                    {
                        var identity = new ClaimsIdentity("TestCookieAuthentication");
                        var principal = new ClaimsPrincipal(identity);

                        context.User = principal;

                        return next();
                    });

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
        }
    }
}
