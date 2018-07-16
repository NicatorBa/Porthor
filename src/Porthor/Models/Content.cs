namespace Porthor.Models
{
    /// <summary>
    /// Settings for content.
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Allowed media type.
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// Schema to validate against a request content.
        /// </summary>
        public string Schema { get; set; }
    }
}
