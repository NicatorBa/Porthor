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
    public class ResourceHandler
    {
        private const string _transferEncodingHeader = "transfer_encoding";

        private readonly IEnumerable<IResourceRequestValidator> _validators;
        private readonly EndpointUriBuilder _uriBuilder;

        public ResourceHandler(
            IEnumerable<IResourceRequestValidator> validators,
            EndpointUriBuilder uriBuilder)
        {
            _validators = validators;
            _uriBuilder = uriBuilder;
        }

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
            using (var responseMessage = await new HttpClient().SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
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

            foreach (var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            context.Response.Headers.Remove(_transferEncodingHeader);
            await responseMessage.Content.CopyToAsync(context.Response.Body);
        }
    }
}
