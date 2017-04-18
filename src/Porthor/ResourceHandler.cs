using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Porthor.ContentValidation;
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
        private readonly QueryParameterSettings _queryParameterConfiguration;
        private readonly ICollection<string> _authorizationPolicies;
        private readonly IDictionary<string, IContentValidator> _mediaTypeContentValidators;
        private readonly EndpointUriFactory _endpointUriFactory;

        public ResourceHandler(
            HttpMethod method,
            QueryParameterSettings queryParameterSettings,
            ICollection<string> authorizationPolicies,
            IDictionary<string, IContentValidator> mediaTypeContentValidators,
            EndpointUriFactory endpointUriFactory)
        {
            _method = method;
            _queryParameterConfiguration = queryParameterSettings;
            _authorizationPolicies = authorizationPolicies;
            _mediaTypeContentValidators = mediaTypeContentValidators;
            _endpointUriFactory = endpointUriFactory;
        }

        public async Task HandleRequestAsync(HttpContext context)
        {
            var requiredQueryParameters = _queryParameterConfiguration.QueryParameters.Where(p => p.Required);
            var missingQueryParameter = requiredQueryParameters.Where(p => !context.Request.Query.ContainsKey(p.Name));

            IEnumerable<string> notSupportedQueryParameters = null;
            if (!_queryParameterConfiguration.AdditionalQueryParameters)
            {
                notSupportedQueryParameters = context.Request.Query.Where(p => !_queryParameterConfiguration.QueryParameters.Any(qp => qp.Name.Equals(p.Key))).Select(p => p.Key);
            }

            if (missingQueryParameter.Count() > 0 ||
                (notSupportedQueryParameters != null && notSupportedQueryParameters.Count() > 0))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            if (_authorizationPolicies.Count > 0)
            {
                IAuthorizationService authorizationService = (IAuthorizationService)context.RequestServices.GetService(typeof(IAuthorizationService));

                if (authorizationService == null)
                {
                    throw new ArgumentNullException(nameof(authorizationService));
                }

                bool authorized = false;
                foreach (var policy in _authorizationPolicies)
                {
                    if (await authorizationService.AuthorizeAsync(context.User, policy))
                    {
                        authorized = true;
                        break;
                    }
                }

                if (!authorized)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            if (_mediaTypeContentValidators.ContainsKey(context.Request.ContentType))
            {
                var validator = _mediaTypeContentValidators[context.Request.ContentType];
                if (!await validator.Validate(context.Request))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }
            }
            else
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
