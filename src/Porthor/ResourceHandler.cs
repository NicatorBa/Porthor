using Microsoft.AspNetCore.Http;
using Porthor.EndpointUri;
using Porthor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor
{
    public class ResourceHandler
    {
        private readonly static string _transferEncodingHeader = "transfer-encoding";

        private readonly HttpMethod _method;
        private readonly QueryParameterConfiguration _queryParameterConfiguration;
        private readonly EndpointUriFactory _endpointUriFactory;

        public ResourceHandler(
            HttpMethod method,
            QueryParameterConfiguration queryParameterConfiguration,
            EndpointUriFactory endpointUriFactory)
        {
            _method = method;
            _queryParameterConfiguration = queryParameterConfiguration;
            _endpointUriFactory = endpointUriFactory;
        }

        public async Task HandleRequestAsync(HttpContext context)
        {
            var requiredQueryParameters = _queryParameterConfiguration.QueryParameters.Where(p => p.Required);
            var missingQueryParameter = requiredQueryParameters.Where(p => !context.Request.Query.ContainsKey(p.FieldName));

            IEnumerable<string> notSupportedQueryParameters = null;
            if (!_queryParameterConfiguration.AdditionalQueryParameters)
            {
                notSupportedQueryParameters = context.Request.Query.Where(p => !_queryParameterConfiguration.QueryParameters.Any(qp => qp.FieldName.Equals(p.Key))).Select(p => p.Key);
            }

            if (missingQueryParameter.Count() > 0 ||
                (notSupportedQueryParameters != null && notSupportedQueryParameters.Count() > 0))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var requestMessage = new HttpRequestMessage();
            var requestMethod = context.Request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod))
            {
                var streamContent = new StreamContent(context.Request.Body);
                requestMessage.Content = streamContent;
            }

            foreach (var header in context.Request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) &&
                    requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            Uri uri = _endpointUriFactory.CreateUri(context);
            requestMessage.Headers.Host = uri.Host;
            requestMessage.RequestUri = uri;
            requestMessage.Method = _method;
            using (var responseMessage = await new HttpClient().SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                foreach (var header in responseMessage.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                foreach (var header in responseMessage.Content.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }

                context.Response.Headers.Remove(_transferEncodingHeader);
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
        }
    }
}
