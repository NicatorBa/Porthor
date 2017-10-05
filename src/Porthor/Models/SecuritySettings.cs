using System.Collections.Generic;

namespace Porthor.Models
{
    /// <summary>
    /// Settings for authentication and authorization to limit access.
    /// </summary>
    public class SecuritySettings
    {
        /// <summary>
        /// A flag indicating if the resource can be used without authentication.
        /// </summary>
        public bool AllowAnonymous { get; set; }

        /// <summary>
        /// Authorization policies for access, only one fulfilled policy is required.
        /// </summary>
        public ICollection<string> Policies { get; set; }
    }
}
