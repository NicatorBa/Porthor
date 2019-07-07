using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Porthor.Validation;

namespace Porthor.Internal
{
    /// <summary>
    /// Handler for specific request.
    /// </summary>
    public class RequestHandler
    {
        private const string TransferEncodingHeader = "transfer-encoding";

        private readonly RequestUriBuilder _uriBuilder;
        private readonly HttpClient _httpClient;
        private readonly IEnumerable<IValidator> _validators;

        /// <summary>
        /// Initialize a new instance of <see cref="RequestHandler"/>.
        /// </summary>
        /// <param name="uriBuilder">Uri builder for a request.</param>
        /// <param name="messageHandler">Message handler for HTTP.</param>
        /// <param name="timeout">The timespan to wait before the request times out.</param>
        /// <param name="validators">Collection of validators.</param>
        public RequestHandler(
            RequestUriBuilder uriBuilder,
            HttpMessageHandler messageHandler,
            TimeSpan? timeout,
            IEnumerable<IValidator> validators)
        {
            _uriBuilder = uriBuilder;
            _httpClient = new HttpClient(messageHandler);
            if (timeout.HasValue)
            {
                _httpClient.Timeout = timeout.Value;
            }
            _validators = validators;
        }

        /// <summary>
        /// Handle the current HTTP request.
        /// </summary>
        /// <param name="context">Current <see cref="HttpContext"/>.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous process.</returns>
        public async Task HandleAsync(HttpContext context)
        {
            foreach (var validator in _validators)
            {
                var validationResult = await validator.ValidateAsync(context);

                if (!validationResult.Succeeded)
                {
                    await SetHttpContextResponse(context, validationResult.ResponseMessage);

                    return;
                }
            }

            var requestMessage = new HttpRequestMessage();
            var requestMethod = context.Request.Method;

            if (HttpMethods.IsPost(requestMethod) ||
                HttpMethods.IsPut(requestMethod))
            {
                requestMessage.Content = new StreamContent(context.Request.Body);
            }

            foreach (var header in context.Request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            var requestUri = _uriBuilder.Build(context);
            requestMessage.Headers.Host = requestUri.Host;
            requestMessage.RequestUri = requestUri;
            requestMessage.Method = new HttpMethod(requestMethod);

            try
            {
                using (var responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
                {
                    await SetHttpContextResponse(context, responseMessage);
                }
            }
            catch (OperationCanceledException)
            {
                await SetHttpContextResponse(context, new HttpResponseMessage(HttpStatusCode.GatewayTimeout));
            }
        }

        private async Task SetHttpContextResponse(HttpContext context, HttpResponseMessage responseMessage)
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

            context.Response.Headers.Remove(TransferEncodingHeader);

            if (responseMessage.Content != null)
            {
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
        }
    }
}
