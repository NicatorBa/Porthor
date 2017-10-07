using Microsoft.AspNetCore.Http;
using Porthor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor.ResourceRequestValidators
{
    /// <summary>
    /// Request validator for a query string.
    /// </summary>
    public class QueryParameterValidator : IResourceRequestValidator
    {
        private readonly QueryParameterSettings _settings;

        /// <summary>
        /// Constructs a new instance of <see cref="QueryParameterValidator"/>.
        /// </summary>
        /// <param name="settings">Settings for query parameters.</param>
        public QueryParameterValidator(QueryParameterSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Validates the query string of the current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">Current context.</param>
        /// <returns>
        /// The <see cref="Task{HttpRequestMessage}"/> that represents the asynchronous query string validation process.
        /// Returns null if the query string is valid.
        /// </returns>
        public Task<HttpResponseMessage> ValidateAsync(HttpContext context)
        {
            var missingQueryParameters = _settings.QueryParameters
                .Where(p => p.Required)
                .Where(p => !context.Request.Query.ContainsKey(p.Name));

            IEnumerable<string> unsupportedQueryParameters = null;
            if (!_settings.AdditionalQueryParameters)
            {
                unsupportedQueryParameters = context.Request.Query
                    .Where(p => !_settings.QueryParameters.Any(qp => qp.Name.Equals(p.Key)))
                    .Select(p => p.Key);
            }

            if (missingQueryParameters.Count() > 0 ||
                (unsupportedQueryParameters != null && unsupportedQueryParameters.Count() > 0))
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return Task.FromResult(responseMessage);
            }

            return Task.FromResult<HttpResponseMessage>(null);
        }
    }
}
