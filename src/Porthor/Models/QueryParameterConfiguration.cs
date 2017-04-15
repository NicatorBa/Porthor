using System.Collections.Generic;

namespace Porthor.Models
{
    public class QueryParameterConfiguration
    {
        public ICollection<QueryParameter> QueryParameters { get; set; }

        public bool AdditionalQueryParameters { get; set; }
    }
}
