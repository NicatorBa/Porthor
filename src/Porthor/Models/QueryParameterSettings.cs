using System.Collections.Generic;

namespace Porthor.Models
{
    /// <summary>
    /// Settings to limit allowed query parameters.
    /// </summary>
    public class QueryParameterSettings
    {
        /// <summary>
        /// Collection of accepted query parameters.
        /// </summary>
        public ICollection<QueryParameter> QueryParameters { get; set; }

        /// <summary>
        /// A flag indicating if unknown query parameters are allowed.
        /// </summary>
        public bool AdditionalQueryParameters { get; set; }
    }
}
