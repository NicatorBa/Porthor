using System;
using System.Collections.Generic;
using System.Linq;
using Porthor.Validation;

namespace Porthor.Configuration
{
    /// <summary>
    /// Content options.
    /// </summary>
    public class ContentOptions
    {
        private readonly IDictionary<string, Type> _validators = new Dictionary<string, Type>();

        /// <summary>
        /// A flag indicating whether content validation is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Add a validator for the specified media type.
        /// </summary>
        /// <typeparam name="T">The content validator.</typeparam>
        /// <param name="mediaType">Media type for which the validator is to be used.</param>
        public void Add<T>(string mediaType)
            where T : ContentValidator
        {
            _validators.Add(mediaType, typeof(T));
        }

        internal ContentValidator CreateContentValidator(string mediaType, string schema)
        {
            var type = _validators.SingleOrDefault(kvp => kvp.Key.Equals(mediaType)).Value;
            if (type == null)
            {
                throw new NotSupportedException(mediaType);
            }

            return (ContentValidator)Activator.CreateInstance(type, schema);
        }
    }
}
