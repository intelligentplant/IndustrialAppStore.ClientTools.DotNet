# IntelligentPlant.IndustrialAppStore.Authentication

This package provides ASP.NET Core services and authentication handlers that enable you to integrate applications with the [Industrial App Store](https://appstore.intelligentplant.com).

Please consider using the [IntelligentPlant.IndustrialAppStore.Templates](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.Templates) package to simplify creation of projects that are pre-configured to use this package.


# Getting Started


## Authentication and Industrial App Store Integration

First, visit the [Industrial App Store](https://appstore.intelligentplant.com) and create a registration for your app. When you register your app, you can configure the default scopes (i.e. permissions) that the app will request (user info, reading user data sources, etc). 

You must also register a redirect URL to use when signing users in. The default relative path used is `/auth/signin-ip` i.e. if your app will run at `https://localhost:44321`, you must register `https://localhost:44321/auth/signin-ip` as an allowed redirect URL.

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


## Calling Industrial App Store APIs

Inject the `IndustrialAppStoreHttpClient` service into your types to obtain an API client that will authenticate as the calling user:

```csharp
app.MapGet("/api/user", async (IndustrialAppStoreHttpClient client) => {
    var user = await client.UserInfo.GetUserInfoAsync();
    return Results.Json(user);
});
```

Refer to the [project repository](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet) for more details on available API calls.
