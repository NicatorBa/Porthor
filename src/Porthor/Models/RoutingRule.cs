using System;
using System.Net.Http;

namespace Porthor.Models
{
    /// <summary>
    /// Represents a rule for routing to the backend API.
    /// </summary>
    public class RoutingRule
    {
        /// <summary>
        /// Frontend path to access the API.
        /// </summary>
        public string FrontendPath { get; set; }

        /// <summary>
        /// Supported HTTP method.
        /// </summary>
        public HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// The time in seconds to wait before the request times out. Default is 100s.
        /// </summary>
        public int? Timeout
        {
            get => _timeout;
            set
            {
                if (value <= 0 || value >= _maxTimeout)
                {
                    throw new ArgumentOutOfRangeException(nameof(Timeout));
                }
                _timeout = value;
            }
        }
        private int? _timeout;
        private const int _maxTimeout = int.MaxValue / 1000;

        /// <summary>
        /// Backend URL to which the request is sent.
        /// </summary>
        public string BackendUrl { get; set; }

        /// <summary>
        /// Validation settings.
        /// </summary>
        public ValidationSettings ValidationSettings { get; set; }
    }
}
