namespace Porthor
{
    /// <summary>
    /// Security configuration options for <see cref="Microsoft.AspNetCore.Builder.PorthorMiddleware"/>.
    /// </summary>
    public class SecurityOptions
    {
        /// <summary>
        /// Flag indicating whether authentication validations should be used.
        /// </summary>
        public bool AuthenticationValidationEnabled { get; set; }

        /// <summary>
        /// Flag indicating whether authorization validations should be used.
        /// </summary>
        public bool AuthorizationValidationEnabled { get; set; }
    }
}
