using Microsoft.AspNetCore.Routing;

namespace Porthor.Internal
{
    /// <summary>
    /// Implementation to get a static uri part.
    /// </summary>
    public class StaticUriPartAccessor : IRequestUriPartAccessor
    {
        private readonly string _value;

        /// <summary>
        /// Creates a new instance of <see cref="StaticUriPartAccessor"/>.
        /// </summary>
        /// <param name="value">The value of the uri part.</param>
        public StaticUriPartAccessor(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Extends the <see cref="StaticUriPartAccessor"/> with given uri part.
        /// </summary>
        /// <param name="value">The value of the uri part.</param>
        /// <returns>A new <see cref="StaticUriPartAccessor"/>.</returns>
        public StaticUriPartAccessor Extend(string value)
        {
            return new StaticUriPartAccessor(_value + value);
        }

        /// <inheritdoc />
        public string GetUriPart(RouteValueDictionary routeValues)
        {
            return _value;
        }
    }
}
