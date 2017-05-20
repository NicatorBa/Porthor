using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Porthor.EndpointUri
{
    public class EndpointUriBuilder
    {
        private readonly IEnumerable<IEndpointUriSection> _sections;

        private EndpointUriBuilder(IEnumerable<IEndpointUriSection> sections)
        {
            _sections = sections;
        }

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
