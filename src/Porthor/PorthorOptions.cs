using Porthor.ContentValidation;
using System;
using System.Collections.Generic;

namespace Porthor
{
    public class PorthorOptions
    {
        private readonly IDictionary<string, ContentValidatorFactory> _validatorFactories = new Dictionary<string, ContentValidatorFactory>();

        public PorthorOptions AddContentValidatorFactory(string mediaType, ContentValidatorFactory factory)
        {
            if (_validatorFactories.ContainsKey(mediaType))
            {
                throw new NotSupportedException();
            }

            _validatorFactories.Add(mediaType, factory);
            return this;
        }

        internal IDictionary<string, ContentValidatorFactory> ContentValidatorFactories { get { return _validatorFactories; } }
    }
}
