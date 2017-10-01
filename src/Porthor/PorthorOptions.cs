namespace Porthor
{
    /// <summary>
    /// Configuration options for <see cref="Microsoft.AspNetCore.Builder.PorthorMiddleware"/>.
    /// </summary>
    public class PorthorOptions
    {
        /// <summary>
        /// Indicator that indicates whether the query string is to be validated.
        /// </summary>
        public bool QueryStringValidationEnabled { get; set; }

        /// <summary>
        /// Security configuration options.
        /// </summary>
        public SecurityOptions Security { get; private set; } = new SecurityOptions();

        /// <summary>
        /// Request content configuration options.
        /// </summary>
        public ContentOptions Content { get; private set; } = new ContentOptions();
    }
}
