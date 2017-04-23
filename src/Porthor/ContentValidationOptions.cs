using Porthor.ResourceRequestValidators.ContentValidators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Porthor
{
    public class ContentValidationOptions
    {
        private readonly IDictionary<string, Type> _validators = new Dictionary<string, Type>();

        public bool Enabled { get; set; }

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
