using System.Collections.Generic;

namespace Porthor.Models
{
    /// <summary>
    /// Allowed query parameters.
    /// </summary>
    public class QueryString
    {
        /// <summary>
        /// Collection of accepted query parameters.
        /// </summary>
        public IEnumerable<QueryParameter> QueryParameters { get; set; }

        /// <summary>
        /// A flag indicating if unknown query parameters are allowed.
        /// </summary>
        public bool AdditionalQueryParameters { get; set; }
    }
}
