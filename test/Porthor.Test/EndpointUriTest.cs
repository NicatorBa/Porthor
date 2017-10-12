using Microsoft.AspNetCore.Routing;
using Porthor.EndpointUri;
using Xunit;

namespace Porthor.Test
{
    public class EndpointUriSectionTest
    {
        [Fact]
        public void CreateSection_ReturnString()
        {
            // Arrange
            var text = "string";
            var section = new EndpointUriSection(text);

            // Act
            var result = section.CreateSection(null);

            // Assert
            Assert.Equal(text, result);
        }
    }

    public class RouteValueSectionTest
    {
        [Fact]
        public void CreateSection_ReturnString()
        {
            // Arrange
            var key = "key";
            var text = "text";
            var dict = new RouteValueDictionary();
            dict.Add(key, text);
            var section = new RouteValueSection(key);

            // Act
            var result = section.CreateSection(dict);

            // Assert
            Assert.Equal(text, result);
        }
    }
}
