# IntelligentPlant.IndustrialAppStore.Authentication

This project defines extension methods for adding authentication to an ASP.NET Core 2.x or 3.x application using the Intelligent Plant [Industrial App Store](https://appstore.intelligentplant.com).


# Installation

Install the [IntelligentPlant.IndustrialAppStore.Authentication](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Authentication) package by following the instructions on NuGet.org.


# Quick Start

_The simplest way to use this library is to create an ASP.NET Core app via the Industrial App Store template, as described [here](/src/IntelligentPlant.IndustrialAppStore.Templates)._

Firstly, register your application with the [Industrial App Store](https://appstore.intelligentplant.com). When you register your app, you can configure the default scopes that your app will request (user info, reading user data sources, etc). You must also register a redirect URL to use when signing users in. The default relative path used is `/auth/signin-ip` i.e. if your app will run at `https://localhost:44321`, you must register `https://localhost:44321/auth/signin-ip` as an allowed redirect URL.

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
    services.AddIndustrialAppStoreAuthentication(options => {
        // Bind the settings from the app configuration to the Industrial App Store 
        // authentication options.
        Configuration.GetSection("IAS").Bind(options);
    });

    // Other configuration
}
```

If your app has a login page that requires the user to e.g. accept a privacy policy or explicitly enable persistent cookies, you can specify this as follows:

```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddIndustrialAppStoreAuthentication(options => {
        // Bind the settings from the app configuration to the Industrial App Store 
        // authentication options.
        Configuration.GetSection("IAS").Bind(options);
        // Set the login path to be our login page.
        options.LoginPath = new PathString("/Account/Login");
    });

    // Other configuration
}
```

Finally, in the `Configure` method in your `Startup` class, add authentication and authorization into your request pipeline:

```csharp
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
```


# PKCE

If you are using ASP.NET Core 3.0 or greater, [PKCE](https://oauth.net/2/pkce/) support is automatically enabled.


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

Refer to the [API client documentation](/docs/data-core-api-client) for more details on available API calls.
