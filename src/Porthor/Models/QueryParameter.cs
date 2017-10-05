namespace Porthor.Models
{
    /// <summary>
    /// Single allowed query parameter.
    /// </summary>
    public class QueryParameter
    {
        /// <summary>
        /// Query parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A flag indicating if query parameter is required.
        /// </summary>
        public bool Required { get; set; }
    }
}
