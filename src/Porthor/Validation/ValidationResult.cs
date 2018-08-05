using System;
using System.Net;
using System.Net.Http;

namespace Porthor.Validation
{
    /// <summary>
    /// Represents the result of a validation.
    /// </summary>
    public class ValidationResult
    {
        private HttpResponseMessage _responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);

        /// <summary>
        /// Flag indicating whether if the validation succeeded or not.
        /// </summary>
        /// <value>True if the validation succeeded, otherwise false.</value>
        public bool Succeeded { get; private set; }

        /// <summary>
        /// The <see cref="HttpResponseMessage"/> which should be returned if the operation failed.
        /// </summary>
        public HttpResponseMessage ResponseMessage => !Succeeded ? _responseMessage : throw new InvalidOperationException();

        /// <summary>
        /// Returns a <see cref="ValidationResult"/> indicating a successful validation operation.
        /// </summary>
        public static ValidationResult Success { get; } = new ValidationResult { Succeeded = true };

        /// <summary>
        /// Creates a <see cref="ValidationResult"/> indicating a failed validation operation.
        /// </summary>
        /// <param name="statusCode">A <see cref="HttpStatusCode"/> for an <see cref="HttpResponseMessage"/> which is returned if the operation failed.</param>
        /// <returns>A <see cref="ValidationResult"/> indicating a failed validation operation.</returns>
        public static ValidationResult Failed(HttpStatusCode statusCode)
        {
            var responseMessage = new HttpResponseMessage(statusCode);

            return Failed(responseMessage);
        }

        /// <summary>
        /// Creates a <see cref="ValidationResult"/> indicating a failed validation operation.
        /// </summary>
        /// <param name="responseMessage">An <see cref="HttpResponseMessage"/> which is returned if the operation failed.</param>
        /// <returns>A <see cref="ValidationResult"/> indicating a failed validation operation.</returns>
        public static ValidationResult Failed(HttpResponseMessage responseMessage = null)
        {
            var result = new ValidationResult { Succeeded = false };

            if (responseMessage != null)
            {
                result._responseMessage = responseMessage;
            }

            return result;
        }
    }
}
