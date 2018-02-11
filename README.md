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
        services.AddPorthor(options =>
        {
            // Activate query string checking
            // Offers a more strict way to handle query params for resources
            options.QueryStringValidationEnabled = true;

            // Enable checking if user is authenticated by default
            // Can be overridden for certain resources with their property `AllowAnonymous`
            options.Security.AuthenticationValidationEnabled = true;

            // Protect your API with policiy-based authorization
            options.Security.AuthorizationValidationEnabled = true;

            // Enable content body validation for POST and PUT methods
            options.Content.ValidationEnabled = true;
            // Register content validator for specific media type
            options.Content.Add<JsonValidator>("application/json");
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        // Minimal configured resource
        // Method: supported Http request method
        // Path: your application api endpoint
        // EndpointUrl: endpoint where the request is to be redirected
        var sampleResource = new Resource {
            Method = HttpMethod.Get,
            Path = "api/samples",
            EndpointUrl = "http://example.org/api/v1/samples"
        }

        // Use Porthor with predefined resource
        app.UsePorthor(new[] { sampleResource });
    }
}
```

## Roadmap

- [ ] Build in XML schema validation
- [ ] Web UI to register, update and delete resource routes
- [ ] Wiki & API documentation
