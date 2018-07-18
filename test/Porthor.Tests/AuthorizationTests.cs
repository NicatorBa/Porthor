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
                        options.AddPolicy("InvalidPolicy", policy => policy.RequireAssertion(context => false));
                    });

                    services.AddPorthor()
                        .EnableAuthorizationValidation();
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
                            Authorization = new Models.Authorization
                            {
                                Policies = new List<string> { "InvalidPolicy" }
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
            Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Request_WithValidPolicy_ReturnsOk()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy("ValidPolicy", policy => policy.RequireAssertion(context => true));
                    });

                    services.AddPorthor()
                        .AddMessageHandler(new TestMessageHandler
                        {
                            Sender = (request, cancellationToken) =>
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                return response;
                            }
                        })
                        .EnableAuthorizationValidation();
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
                            Authorization = new Models.Authorization
                            {
                                Policies = new List<string> { "ValidPolicy" }
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
    }
}
