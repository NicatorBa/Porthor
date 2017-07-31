using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Porthor.Models;
using System.Net.Http;

namespace SimpleGatewaySample
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup()
        {
            _configuration = new ConfigurationBuilder()
                                    .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPorthor(_configuration, options =>
            {
                options.QueryStringValidationEnabled = false;
                options.Security.AuthenticationValidationEnabled = false;
                options.Security.AuthorizationValidationEnabled = false;
                options.Content.ValidationEnabled = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var resource = new Resource
            {
                Name = "OpenStreetMapMapData",  // see http://wiki.openstreetmap.org/wiki/API_v0.6#Retrieving_map_data_by_bounding_box:_GET_.2Fapi.2F0.6.2Fmap for detailed api documentation
                Method = HttpMethod.Get,
                Path = "map",                   // call http://localhost:5000/map?bbox=0,10,0,12 when sample is running
                EndpointUrl = "http://api.openstreetmap.org/api/0.6/map"
            };

            app.UsePorthor(new[] { resource });
        }
    }
}
