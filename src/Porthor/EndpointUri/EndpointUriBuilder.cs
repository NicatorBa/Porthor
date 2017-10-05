using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Porthor.EndpointUri
{
    /// <summary>
    /// Builder for a specific endpoint uri.
    /// </summary>
    public class EndpointUriBuilder
    {
        private readonly IEnumerable<IEndpointUriSection> _sections;

        private EndpointUriBuilder(IEnumerable<IEndpointUriSection> sections)
        {
            _sections = sections;
        }

        /// <summary>
        /// Initialize an instance of <see cref="EndpointUriBuilder"/>.
        /// </summary>
        /// <param name="endpointUrl">The template for the endpoint url.</param>
        /// <param name="config">Application configuration.</param>
        /// <returns>A new instance of <see cref="EndpointUriBuilder"/>.</returns>
        public static EndpointUriBuilder Initialize(string endpointUrl, IConfiguration config)
        {
            var splitRegex = new Regex(@"(?=[{[])|(?<=[}\]])");
            var matchEnvRegex = new Regex(@"(?<=\[).*(?=])");
            var matchRouteRegex = new Regex(@"(?<={).*(?=})");

            var uriSections = new List<IEndpointUriSection>();
            string[] sections = splitRegex.Split(endpointUrl);
            for (int i = 0; i < sections.Length; i++)
            {
                var envMatch = matchEnvRegex.Match(sections[i]);
                var routeMatch = matchRouteRegex.Match(sections[i]);
                if (envMatch.Success)
                {
                    uriSections.Add(new EndpointUriSection(config.GetValue<string>(envMatch.Value)));
                }
                else if (routeMatch.Success)
                {
                    uriSections.Add(new RouteValueSection(routeMatch.Value));
                }
                else
                {
                    uriSections.Add(new EndpointUriSection(sections[i]));
                }
            }

            return new EndpointUriBuilder(uriSections);
        }

        /// <summary>
        /// Build the endpoint uri for the current context.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <returns>The <see cref="Uri"/> of an endpoint.</returns>
        public Uri Build(HttpContext context)
        {
            var builder = new StringBuilder();

            var routeValues = context.GetRouteData().Values;
            foreach (var section in _sections)
            {
                builder.Append(section.CreateSection(routeValues));
            }

            if (context.Request.QueryString.HasValue)
            {
                builder.Append(context.Request.QueryString.Value);
            }

            return new Uri(builder.ToString());
        }
    }
}
