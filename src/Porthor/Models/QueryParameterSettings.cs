using System.Collections.Generic;

namespace Porthor.Models
{
    public class QueryParameterSettings
    {
        public ICollection<QueryParameter> QueryParameters { get; set; }

        public bool AdditionalQueryParameters { get; set; }
    }
}
