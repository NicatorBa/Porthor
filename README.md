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
        services.AddPorthor(_configuration, options =>
        {
            // Your gateway configuration
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UsePorthor();
    }
}
```
