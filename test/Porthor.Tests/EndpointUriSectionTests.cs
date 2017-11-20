using Porthor.EndpointUri;
using Xunit;

namespace Porthor.Test
{
    public class EndpointUriSectionTests
    {
        [Fact]
        public void CreateSection_InitializeWithText_ReturnThisText()
        {
            // Arrange
            var text = "sectionText";
            var section = new EndpointUriSection(text);

            // Act
            var result = section.CreateSection(null);

            // Assert
            Assert.Equal(text, result);
        }
    }
}
