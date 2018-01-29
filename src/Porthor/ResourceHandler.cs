using Microsoft.AspNetCore.Http;
using Porthor.EndpointUri;
using Porthor.ResourceRequestValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Porthor
{
    /// <summary>
    /// Handler for specific resource request.
    /// </summary>
    public class ResourceHandler
    {
        private const string _transferEncodingHeader = "transfer-encoding";

        private readonly IEnumerable<IResourceRequestValidator> _validators;
        private readonly EndpointUriBuilder _uriBuilder;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructs a new instance of <see cref="ResourceHandler"/>.
        /// </summary>
        /// <param name="validators">Collection of validators.</param>
        /// <param name="uriBuilder">Builder for endpoint uri.</param>
        /// <param name="httpMessageHandler">Message handler for HTTP.</param>
        public ResourceHandler(
            IEnumerable<IResourceRequestValidator> validators,
            EndpointUriBuilder uriBuilder,
            HttpMessageHandler httpMessageHandler = null)
        {
            _validators = validators;
            _uriBuilder = uriBuilder;
            _httpClient = new HttpClient(httpMessageHandler ?? new HttpClientHandler());
        }

        /// <summary>
        /// Handle the current HTTP request.
        /// </summary>
        /// <param name="context">Current <see cref="HttpContext"/>.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous process.</returns>
        public async Task HandleRequestAsync(HttpContext context)
        {
            foreach (var validator in _validators)
            {
                var validatorResponseMessage = await validator.ValidateAsync(context);
                if (validatorResponseMessage != null)
                {
                    await SendResponse(context, validatorResponseMessage);
                    return;
                }
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
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            Uri uri = _uriBuilder.Build(context);
            requestMessage.Headers.Host = uri.Host;
            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(requestMethod);
            using (var responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
            {
                await SendResponse(context, responseMessage);
            }
        }

        private async Task SendResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            context.Response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            if (responseMessage.Content != null)
            {
                foreach (var header in responseMessage.Content.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }
            }

            context.Response.Headers.Remove(_transferEncodingHeader);
            if (responseMessage.Content != null)
            {
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
        }
    }
}
