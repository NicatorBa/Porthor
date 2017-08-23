# Porthor
**What's Porthor?**

Porthor is an API gateway middleware for Asp.Net Core. It offers an easy to use way to register APIs from other sources to your application. You can also protect your API by using ASP.NET Core policy-based authorization.

## Getting started

- **Update your `.csproj` file** to reference `Porthor`package

- **Configure Porthor service** in `Startup`:

```csharp
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup()
    {
        // Add your configuration sources
        _configuration = new ConfigurationBuilder()
                                .Build();
    }

    public void ConfigureServices(IServiceCollection service)
    {
        // Register the Porthor services
        // with your gateway configuration
        services.AddPorthor(_configuration, options =>
        {
            // Activate query string checking
            // Offers a more strict way to handle query params for resources
            options.QueryStringValidationEnabled = true;

            // Enable checking if user is authenticated by default
            // Can be overridden for certain resources with their property `AllowAnonymous`
            options.Security.AuthenticationValidationEnabled = true;

            // Protect your API with policiy-based authorization
            options.Security.AuthorizationValidationEnabled =true;

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

- [ ] Documentation comments
- [ ] Tests
- [ ] Management by Web UI
- [ ] Wiki
- [ ] XML schema validation
- [ ] Support for .NET Core 2.0
