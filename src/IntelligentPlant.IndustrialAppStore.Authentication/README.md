# IntelligentPlant.IndustrialAppStore.Authentication

This project defines extension methods for adding authentication to an ASP.NET Core application using the Intelligent Plant [Industrial App Store](https://appstore.intelligentplant.com).


# Installation

Install the [IntelligentPlant.IndustrialAppStore.Authentication](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Authentication) package by following the instructions on NuGet.org.


# Quick Start

> **NOTE:**
> The simplest way to use this library is to create an ASP.NET Core app via the Industrial App Store template, as described [here](/src/IntelligentPlant.IndustrialAppStore.Templates).

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

> **NOTE:**
> Do not store client secrets in the `appsettings.json` file in a production environment! Services such as [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/) can be used to securely store client secrets and retrieve them at runtime.

Next, configure your application to use the Industrial App Store for authentication:

```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIndustrialAppStoreAuthentication(options => {
    // Bind the settings from the app configuration to the Industrial App Store 
    // authentication options.
    builder.Configuration.GetSection("IAS").Bind(options);
});

// Configure additional services etc. here.

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Configure HTTP pipeline here.
```

If your app has a login page that requires the user to accept a privacy policy or explicitly enable persistent cookies, you can specify this as follows:

```csharp
builder.Services.AddIndustrialAppStoreAuthentication(options => {
    // Bind the settings from the app configuration to the Industrial App Store 
    // authentication options.
    builder.Configuration.GetSection("IAS").Bind(options);

    // Set the login path to be our login page.
    options.LoginPath = new PathString("/Account/Login");
});
```


# Calling Industrial App Store APIs

Refer to the [API client documentation](/docs/data-core-api-client) for more details on available API calls.


## Which Industrial App Store Client Should I Use?

The `IntelligentPlant.IndustrialAppStore.Authentication` package registers two different Industrial App Store client types with your application's dependency injection container:

- [IndustrialAppStoreHttpClient](./IndustrialAppStoreHttpClient.cs)
- [BackchannelIndustrialAppStoreHttpClient](./BackchannelIndustrialAppStoreHttpClient.cs)

The two clients function identically, but use different types for the context parameter on each operation. The guidance for choosing which client to use is as follows:


**When making calls to the Industrial App Store from your app's HTTP request pipeline, use IndustrialAppStoreHttpClient**

`IndustrialAppStoreHttpClient` expects you to pass an `HttpContext` object in each method call. The access token for the calling user is automatically retrieved from the `HttpContext` and appended to the HTTP headers for outgoing Industrial App Store requests:

```csharp
[ApiController]
public class ExampleController : ControllerBase {

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetIndustrialAppStoreDataSources(
        [FromServices] IndustrialAppStoreHttpClient iasClient,
        CancellationToken cancellationToken
    ) {
        var hasAccessToken = await IndustrialAppStoreHttpClient.HasValidAccessToken(Request.HttpContext);
        if (!hasAccessToken) {
            // User does not have a valid access token.
            return NotAuthorized();
        }

        var dataSources = await iasClient.DataSources.GetDataSourcesAsync(Request.HttpContext, cancellationToken);
        return Ok(dataSources);
    }

}
```

**When making calls to the Industrial App Store from a background task or an IHostedService, use BackchannelIndustrialAppStoreHttpClient**

`BackchannelIndustrialAppStoreHttpClient` expects an [ITokenStore](./ITokenStore.cs) to be passed when making calls to the Industrial App Store. The access token is retrieved from the `ITokenStore` and appended to the outgoing HTTP request headers. 

This allows you to make calls to the Industrial App Store from outside the app's HTTP request pipeline, such as from a background task or an `IHostedService`. For example, your app might allow a user to configure a periodic task that will perform some anaylsis on an industrial process in the background, regardless of whether or not the user has the app open in their browser.

Note that this scenario requires that you are using a custom `ITokenStore` implementation in your app, so that you can persist access tokens to a location other than the browser's session cookie. See 'Using a Custom Token Store' below for details.

```csharp
public class MyAnalysis {

    private readonly IServiceProvider _serviceProvider;

    public MyAnalysis(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public async Task RunAnalysisForUserAsync(string userId, string sessionId, CancellationToken cancellationToken) {
        // ITokenStore is a scoped service, so we need to create a scope for this operation.
        using (var scope = _serviceProvider.CreateScope()) {
            var iasClient = scope.ServiceProvider.GetRequiredService<BackchannelIndustrialAppStoreHttpClient>();
            var tokenStore = scope.ServiceProvider.GetRequiredService<ITokenStore>();

            // We need to initialise ITokenStore ourselves when using it outside of the HTTP 
            // pipeline.
            await tokenStore.InitAsync(userId, sessionId);

            var dataSources = await iasClient.DataSources.GetDataSourcesAsync(tokenStore, cancellationToken);

            // TODO: Use iasClient to retrieve required data and perform analysis.
        }
    }

}
```

It is also possible to use `BackchannelIndustrialAppStoreHttpClient` from inside the HTTP request pipeline if preferred. In this situation, you must manually retrieve the `ITokenStore` for the authenticated user from the current `HttpContext`. The equivalent API controller code above using `BackchannelIndustrialAppStoreHttpClient` instead of `IndustrialAppStoreHttpClient` would be as follows:


```csharp
[ApiController]
public class ExampleController : ControllerBase {

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetIndustrialAppStoreDataSources(
        [FromServices] BackchannelIndustrialAppStoreHttpClient iasClient,
        CancellationToken cancellationToken
    ) {
        var tokenStore = Request.HttpContext.RequestServices.GetRequiredService<ITokenStore>();
        var hasAccessToken = await BackchannelIndustrialAppStoreHttpClient.HasValidAccessToken(tokenStore);
        if (!hasAccessToken) {
            // User does not have a valid access token.
            return NotAuthorized();
        }

        var dataSources = await iasClient.DataSources.GetDataSourcesAsync(tokenStore, cancellationToken);
        return Ok(dataSources);
    }

}
```



# Using a Custom Token Store

The [ITokenStore](./ITokenStore.cs) service is used to store and retrieve Industrial App Store access tokens for calling users. The [default implementation](./DefaultTokenStore.cs) uses the ASP.NET Core `AuthenticationProperties` from the calling user's authentication ticket to hold the user's access token, meaning that the token is stored in the browser's session cookie.

Under many circumstances, this approach is perfectly valid. However, if your application performs some sort of background data processing on behalf of a user (e.g. an app that periodically retrieves historian data, performs some calculations, and then writes the calculation results back to an Edge Historian), you will need to provide a custom `ITokenStore` implementation so that access tokens (and refresh tokens) can be persisted to somewhere other than a user's browser session cookie.

> **NOTE:**
> Access tokens and refresh tokens should be treated with the same level of security as passwords. They should never be persisted without being encrypted first.

When writing a custom `ITokenStore`, you should inherit from the abstract [TokenStore](./TokenStore.cs) base class.

An example project showing how to write a custom `ITokenStore` implementation that uses Entity Framework Core to persist tokens to a SQLite database can be found [here](../../samples/CustomTokenStoreExample).

Register your custom token store when adding Industrial App Store authentication to your project as follows:

```csharp
services.AddIndustrialAppStoreAuthentication<MyCustomTokenStore>(options => {
    // Bind the settings from the app configuration to the Industrial App Store 
    // authentication options.
    Configuration.GetSection("IAS").Bind(options);
    // Set the login path to be our login page.
    options.LoginPath = new PathString("/Account/Login");
});
```

You can also provide a factory function to create the token store instance:

```csharp
services.AddIndustrialAppStoreAuthentication<MyCustomTokenStore>(
    options => {
        // Bind the settings from the app configuration to the Industrial App Store 
        // authentication options.
        Configuration.GetSection("IAS").Bind(options);
        // Set the login path to be our login page.
        options.LoginPath = new PathString("/Account/Login");
    }, 
    (serviceProvider, httpClient) => ActivatorUtils.CreateInstance<MyCustomTokenStore>(serviceProvider, httpClient)
);
```

> The `ITokenStore` service is always registered as a *scoped* service.
