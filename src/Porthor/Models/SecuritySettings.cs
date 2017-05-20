using System.Collections.Generic;

namespace Porthor.Models
{
    public class SecuritySettings
    {
        public bool AllowAnonymous { get; set; }

        public ICollection<string> Policies { get; set; }
    }
}
