using Porthor.ResourceRequestValidators.ContentValidators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Porthor
{
    /// <summary>
    /// Content configuration options for <see cref="Microsoft.AspNetCore.Builder.PorthorMiddleware"/>.
    /// </summary>
    public class ContentOptions
    {
        private readonly IDictionary<string, Type> _validators = new Dictionary<string, Type>();

        /// <summary>
        /// Flag indicating whether the content is being validated.
        /// </summary>
        public bool ValidationEnabled { get; set; } = false;

        /// <summary>
        /// Add a validator for the specified media type.
        /// </summary>
        /// <typeparam name="T">The content validator.</typeparam>
        /// <param name="mediaType">Media type for which the validator is to be used.</param>
        public void Add<T>(string mediaType)
            where T : ContentValidatorBase
        {
            _validators.Add(mediaType, typeof(T));
        }

        internal ContentValidatorBase Get(string mediaType, string template)
        {
            var validatorType = _validators.SingleOrDefault(v => v.Key.Equals(mediaType)).Value;
            if (validatorType == null)
            {
                return null;
            }

            return (ContentValidatorBase)Activator.CreateInstance(validatorType, template);
        }
    }
}
