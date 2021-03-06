﻿using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Porthor.Validation.Content
{
    /// <summary>
    /// Validator for xml content.
    /// </summary>
    public class XmlValidator : ContentValidator
    {
        private readonly XmlSchemaSet _xmlSchema;

        /// <summary>
        /// Creates a new instance of <see cref="XmlValidator"/>.
        /// </summary>
        /// <param name="schema">Xml validation schema.</param>
        public XmlValidator(string schema) : base(schema)
        {
            _xmlSchema = new XmlSchemaSet();
            _xmlSchema.Add(string.Empty, XmlReader.Create(new StringReader(Schema)));
        }

        /// <summary>
        /// Validates the xml of current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>The <see cref="Task{ValidationResult}"/> that represents the asynchronous validation process.</returns>
        public override Task<ValidationResult> ValidateAsync(HttpContext context)
        {
            var errors = false;
            var document = XDocument.Load(context.Request.Body);

            document.Validate(_xmlSchema, (o, e) => { errors = true; });

            if (errors)
            {
                return Task.FromResult(ValidationResult.Failed());
            }

            return Task.FromResult(ValidationResult.Success);
        }
    }
}
