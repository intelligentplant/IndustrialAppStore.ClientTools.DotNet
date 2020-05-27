# IntelligentPlant.IndustrialAppStore.Authentication.AspNetCore

This project defines extension methods for adding authentication to an ASP.NET Core 2.x or 3.x application using the Intelligen Plant [Industrial App Store](https://appstore.intelligentplant.com).


# Quick Start

Firstly, register your application with the [Industrial App Store](https://appstore.intelligentplant.com). When you register your app, you can configure the default scopes that your app will request (user info, reading user data sources, etc). You must also register a redirect URL to use when signing users in. The default relative path used is `/auth/signin-ip` i.e. if your app will run at `http://localhost:44321`, you must register `http://localhost:44321/auth/signin-ip` as an allowed redirect URL.

In your application's `appsettings.json` file, add the following items, replacing the placeholders with values from your app registration:

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>",
        "ClientSecret": "<YOUR CLIENT SECRET>"
    }
}
```

In your `Startup` class, add the following namespace import:

```csharp
using IntelligentPlant.IndustrialAppStore.Authentication;
```

In the `ConfigureServices` method in your `Startup` class, configure your application to use the Industrial App Store for authentication. Note that this example assumes that you have made your configuration settings accessible to the `Startup` class [as described here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration#access-configuration-in-startup).

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddAuthentication(options => {
        // If an authentication cookie is present, use it.
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        // If authentication is required, and no cookie is present, use the Industrial App 
        // Store to sign in.
        options.DefaultChallengeScheme = IASAuthDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddIndustrialAppStoreAuthentication(options => {
        // Bind the settings from the app configuration to the Industrial App Store 
        // authentication options.
        Configuration.GetSection("IAS").Bind(options);
    });

    // Other configuration
}
```

Finally, in the `Configure` method in your `Startup` class, add authentication and authorization into you rquest pipeline:

public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
    // Other configuration

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints => {
        // Map endpoint routes here.
    });

    // Other configuration
}


# PKCE

If you are using ASP.NET Core 3.0 or greater, you can enable [PKCE](https://oauth.net/2/pkce/) support instead of using a client secret to sign users in. Note that you must also enable PKCE in your app settings on the Industrial App Store. You can enable PKCE by modifying your application's `appsettings.json` file:

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>",
        "ClientSecret": "<YOUR CLIENT SECRET>",
        "UsePkce": true
    }
} 

Alternatively, you can also enable PKCE programatically from your `Startup` class:

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddAuthentication(options => {
        // Removed for brevity
    })
    .AddCookie()
    .AddIndustrialAppStoreAuthentication(options => {
        // Bind the settings from the app configuration to the Industrial App Store 
        // authentication options.
        Configuration.GetSection("IAS").Bind(options);
        options.UsePkce = true;
    });

    // Other configuration
}
```

# Calling Industrial App Store APIs

The `IndustrialAppStoreHttpClient` service is automatically registered with the dependency injection container to allow you to call Industrial App Store APIs. When calling API methods, you can pass the `HttpContext` for the calling user to automatically use their access token to authorize the call:

```csharp
[ApiController]
public class ExampleController : ControllerBase {

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetIndustrialAppStoreDataSources(
        [FromServices] IndustrialAppStoreHttpClient iasClient,
        CancellationToken cancellationToken
    ) {
        var hasAccessToken = await iasClient.HasValidAccessToken(Request.HttpContext);
        if (!hasAccessToken) {
            // User does not have a valid access token.
            return NotAuthorized();
        }

        var dataSources = await iasClient.DataSources.GetDataSourcesAsync(Request.HttpContext, cancellationToken);
        return Ok(dataSources);
    }

}
```
