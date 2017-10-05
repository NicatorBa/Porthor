namespace Porthor.Models
{
    /// <summary>
    /// Definition for request content, only used if HTTP method is POST or PUT.
    /// </summary>
    public class ContentDefinition
    {
        /// <summary>
        /// Allowed media type for request content.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Template to validate against a request content.
        /// </summary>
        public string Template { get; set; }
    }
}
