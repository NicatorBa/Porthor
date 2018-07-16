namespace Porthor.Models
{
    /// <summary>
    /// Settings for authentication.
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// A flag indicating if the route can be used without authentication.
        /// </summary>
        public bool AllowAnonymous { get; set; }
    }
}
