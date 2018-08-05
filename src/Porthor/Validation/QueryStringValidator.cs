using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Porthor.Validation
{
    /// <summary>
    /// Validator for query string.
    /// </summary>
    public class QueryStringValidator : IValidator
    {
        private readonly Models.QueryString _queryString;

        /// <summary>
        /// Creates a new instance of <see cref="QueryStringValidator"/>.
        /// </summary>
        /// <param name="queryString">Allowed query string settings.</param>
        public QueryStringValidator(Models.QueryString queryString)
        {
            _queryString = queryString;
        }

        /// <summary>
        /// Validates the query string of the current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>The <see cref="Task{ValidationResult}"/> that represents the asynchronous validation process.</returns>
        public Task<ValidationResult> ValidateAsync(HttpContext context)
        {
            var missingQueryParameters = _queryString.QueryParameters
                .Where(p => p.Required)
                .Where(p => !context.Request.Query.ContainsKey(p.Name));

            IEnumerable<string> unsupportedQueryParameters = new string[] { };
            if (!_queryString.AdditionalQueryParameters)
            {
                unsupportedQueryParameters = context.Request.Query
                    .Where(p => !_queryString.QueryParameters.Any(qp => qp.Name.Equals(p.Key, StringComparison.OrdinalIgnoreCase)))
                    .Select(p => p.Key);
            }

            if (missingQueryParameters.Any() || unsupportedQueryParameters.Any())
            {
                return Task.FromResult(ValidationResult.Failed(HttpStatusCode.BadRequest));
            }

            return Task.FromResult(ValidationResult.Success);
        }
    }
}
