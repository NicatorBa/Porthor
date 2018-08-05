using System.Collections.Generic;

namespace Porthor.Models
{
    /// <summary>
    /// Settings for authorization.
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// Policies for access, only on fulfilled policy is required.
        /// </summary>
        public IEnumerable<string> Policies { get; set; }
    }
}
