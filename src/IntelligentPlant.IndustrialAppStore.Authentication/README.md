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

Inject the `IndustrialAppStoreClient` service into your types to obtain an API client that will authenticate as the calling user.

Refer to the [API client documentation](/docs/data-core-api-client) for more details on available API calls.

`IndustrialAppStoreClient` is a _scoped_ service. For scenarios where you need to call the Industrial App Store APIs from a background service or similar, you must ensure that your code runs inside or creates a new dependency injection scope. Additionally, when running outside of the HTTP request pipeline, you must explicitly retrieve and initialise the `ITokenStore` service so that it can be used to provide access tokens to the API client. 

> The `ITokenStore` service is initialised automatically when the client is created within the HTTP request pipeline.


# Using a Custom Token Store

The [ITokenStore](./ITokenStore.cs) service is used to store and retrieve Industrial App Store access tokens for calling users. The [default implementation](./AuthenticationPropertiesTokenStore.cs) uses the ASP.NET Core `AuthenticationProperties` from the calling user's authentication ticket to hold the user's access token, meaning that the token is stored in the browser's session cookie.

Under many circumstances, this approach is perfectly valid. However, if your application performs some sort of background data processing on behalf of a user (e.g. an app that periodically retrieves historian data, performs some calculations, and then writes the calculation results back to an Edge Historian), you will need to provide a custom `ITokenStore` implementation so that access tokens (and refresh tokens) can be persisted to somewhere other than a user's browser session cookie.

> **NOTE:**
> Access tokens and refresh tokens should be treated with the same level of security as passwords. They should never be persisted without being encrypted first.

When writing a custom `ITokenStore`, you should inherit from the abstract [TokenStore](./TokenStore.cs) base class.

An example project showing how to write a custom `ITokenStore` implementation that uses Entity Framework Core to persist tokens to a SQLite database can be found [here](../../samples/CustomTokenStoreExample).

Register your custom token store when adding Industrial App Store authentication to your project as follows:

```csharp
services.AddIndustrialAppStoreAuthentication(options => {
    // Bind the settings from the app configuration to the Industrial App Store 
    // authentication options.
    Configuration.GetSection("IAS").Bind(options);
    // Set the login path to be our login page.
    options.LoginPath = new PathString("/Account/Login");
}).AddTokenStore<MyCustomTokenStore>();
```

You can also provide a factory function to create the token store instance:

```csharp
services.AddIndustrialAppStoreAuthentication(
    options => {
        // Bind the settings from the app configuration to the Industrial App Store 
        // authentication options.
        Configuration.GetSection("IAS").Bind(options);
        // Set the login path to be our login page.
        options.LoginPath = new PathString("/Account/Login");
    }
).AddTokenStore((serviceProvider, httpClient) => ActivatorUtils.CreateInstance<MyCustomTokenStore>(serviceProvider, httpClient));
```

The `HttpClient` instance passed to the token store's constructor is used for backchannel communications with the Industrial App Store authentication server's token endpoint. If your token store inherits from the `TokenStore` base class, the token store will automatically refresh access tokens when they expire and persist the new access token and refresh token to the store.

> The `ITokenStore` service is always registered as a *scoped* service.


# Customising the HTTP Client Registration

To customise the `IHttpClientFactory` HTTP client registration for the `IndustrialAppStoreHttpClient` type, you can use the `AddApiClient` extension method for the `IIndustrialAppStoreBuilder` type:

```csharp
services
    .AddIndustrialAppStoreAuthentication(options => {
        // Bind the settings from the app configuration to the Industrial App Store 
        // authentication options.
        Configuration.GetSection("IAS").Bind(options);
        // Set the login path to be our login page.
        options.LoginPath = new PathString("/Account/Login");
    })
    .AddApiClient(httpBuilder => httpBuilder
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler() {
            EnableMultipleHttp2Connections = true
        })
        .AddStandardResilienceHandler());
```

> The example shown above is for illustration purposes only and is equivalent to the HTTP client registration that is added by default.
