using Microsoft.AspNetCore.Routing;
using Porthor.EndpointUri;
using Xunit;

namespace Porthor.Test
{
    public class RouteValueSectionTests
    {
        [Fact]
        public void CreateSection_InitializeWithKeyName_ReturnTextAssociatedToKey()
        {
            // Arrange
            var key = "textKey";
            var text = "sectionText";
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
