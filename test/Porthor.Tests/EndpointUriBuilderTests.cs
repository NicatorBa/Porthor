using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using Porthor.EndpointUri;
using Xunit;

namespace Porthor.Tests
{
    public class EndpointUriBuilderTests
    {
        [Fact]
        public void BuildUri_InitializeWithUrl_ReturnFixedUrl()
        {
            // Arrange
            var url = "http://example.com/api/values";
            var builder = EndpointUriBuilder.Initialize(url, null);
            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(context => context.Features[typeof(IRoutingFeature)]).Returns(new RoutingFeature { RouteData = new RouteData() });
            mockContext.Setup(context => context.Request.QueryString).Returns(null);

            // Act
            var uri = builder.Build(mockContext.Object);

            // Assert
            Assert.Equal(url, uri.ToString());
        }
    }
}
