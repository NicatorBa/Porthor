namespace Porthor.Models
{
    /// <summary>
    /// Validation settings for a route.
    /// </summary>
    public class Validations
    {
        /// <summary>
        /// Authentication settings.
        /// </summary>
        public Authentication Authentication { get; set; }

        /// <summary>
        /// Authorization settings.
        /// </summary>
        public Authorization Authorization { get; set; }

        /// <summary>
        /// Query string settings.
        /// </summary>
        public QueryString QueryString { get; set; }

        /// <summary>
        /// Content settings.
        /// </summary>
        public Contents Contents { get; set; }
    }
}
