using System.Collections.Generic;

namespace Porthor.Models
{
    public class SecuritySettings
    {
        public ICollection<string> Policies { get; set; }

        public bool AllowAnonymous { get; set; }
    }
}
