# Porthor

[![NuGet](https://img.shields.io/nuget/v/Porthor.svg)](https://www.nuget.org/packages?q=Porthor)
[![GitHub issues](https://img.shields.io/github/issues/NicatorBa/Porthor.svg)](https://github.com/NicatorBa/Porthor/issues)
[![GitHub license](https://img.shields.io/github/license/NicatorBa/Porthor.svg)](https://github.com/NicatorBa/Porthor/blob/master/LICENSE)

**What's Porthor?**

Porthor is an API gateway middleware for ASP.NET Core 2.0. It offers an easy to use way to register APIs from other sources to your application. You can also protect your API by using ASP.NET Core policy-based authorization.

## Getting started

- **Update your `.csproj` file** to reference `Porthor` package

- **Configure Porthor service** in `Startup`:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection service)
    {
        // Register the Porthor services
        // with your gateway configuration
        services.AddPorthor()
            // Enable checking if user is authenticated by default
            // Can be overridden for certain resources with their property `AllowAnonymous`
            .EnableAuthenticationValidation()

            // Protect your API with policiy-based authorization
            .EnableAuthorizationValidation()

            // Activate query string checking
            // Offers a more strict way to handle query params for resources
            .EnableQueryStringValidation()

            .ConfigureContentValidation(options =>
            {
                // Enable content body validation for POST and PUT methods
                options.Enabled = true;
                // Register content validator for specific media type
                options.Add<JsonValidator>(MediaType.Application.Json);
            });
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        // Minimal configured resource
        // HttpMethod: supported Http request method
        // FrontendPath: your application api endpoint
        // BackendUrl: endpoint where the request is to be redirected
        var routingRule = new RoutingRule {
            HttpMethod = HttpMethod.Get,
            FrontendPath = "api/samples",
            BackendUrl = "http://example.org/api/v1/samples"
        }

        // Use Porthor with predefined resource
        app.UsePorthor(new[] { routingRule });
    }
}
```

## Roadmap

- [ ] Build in XML schema validation
- [ ] Web UI to register, update and delete resource routes
- [ ] Wiki & API documentation
