# IntelligentPlant.IndustrialAppStore.AspNetCore

This package provides a set of ASP.NET Core services and middleware that enable you to integrate applications with the [Industrial App Store](https://appstore.intelligentplant.com).

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


## Custom Headers

The custom headers services and middleware allow you to define HTTP response headers via configuration that will be added to every outgoing response:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomHeaders();

// ...

var app = builder.Build();

app.UseCustomHeaders();

// ...

app.Run();
```

You can define the custom response headers via the `CustomHeaders` section of your application's `appsettings.json` file. For example:

```json
{
  "CustomHeaders": {
    "X-Content-Type-Options": "nosniff",
    "X-Frame-Options": "deny",
    "X-XSS-Protection": "1; mode=block"
  }
}
```


## Content Security Policy

A [Content Security Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP) gives you fine-grained control over where the browser can load content such as scripts and images from. For simple use cases, you can set a hard-coded `Content-Security-Policy` response header using the custom headers middleware above. However, the library also provides dedicated middleware and services to define a CSP in a more flexible way.

First, register the required services and middlware with your application:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Next line is only required if you want to define your CSP in a separate configuration file.
builder.Configuration.AddJsonFile("csp.json", optional: true, reloadOnChange: true);

builder.Services.AddContentSecurityPolicy();

// ...

var app = builder.Build();

app.UseContentSecurityPolicy();

// ...

app.Run();
```

Next, define your CSP via the `ContentSecurityPolicy` section of your application's `appsettings.json` file (or in a separate configuration file if preferred). For example:

```json
{
  "ContentSecurityPolicy": {
    "Policies": {
      "_": {
        "Priority": 100,
        "Match": [ "*" ],
        "Policy": {
          "default-src": [
            "'self'"
          ],
          "frame-ancestors": [
            "'none'"
          ],
          "script-src": [
            "'self'"
          ],
          "style-src": [
            "'self'"
          ],
          "img-src": [
            "'self'"
          ]
        }
      },
      "home_controller": {
        "Match": [ "/", "/Home/*" ],
        "Policy": {
          "script-src": [
            "'unsafe-hashes'",
            "'sha256-xzhLGrw7novI3sfqwa1y2oKXixPoY89o+n4dy1X+lWU='"
          ],
          "style-src": [
            "'sha256-0pgtLHffxw9208zzWrww2r1Jt4PeiShv+N72R+PmXAU='"
          ],
          "img-src": [
            "data:",
            "https://appstore.intelligentplant.com"
          ]
        }
      }
    }
  }
}
```

The CSP configuration is mapped to the `ContentSecurityPolicyOptions` options class at runtime. Each value in the `Policies` section is mapped to a `ContentSecurityPolicyDefinition` object, with the key being an identifier for the policy.

An individual policy definition can define the following properties:

- `Match` - An array of URL paths that the policy matches. Entries can use `*` as a wildcard. If not specified, the definition will match all paths.
- `Priority` - An integer priority for the definition. Definitions with higher priorities will be applied first. If not specified, a priority of zero will be used.
- `Policy` - the CSP directives and values for the policy. See [here](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP) for information about the available directives and values.

Multiple policies can be applied to a given path. Policies are applied from highest priority to lowest priority, and are additive by default (i.e. if both of the policies in the example above were applied to a request, the generated CSP would be the composite of both policies).

To remove a directive value added by a policy with a higher priority, prefix the value with `-:`. For example:

```json
{
  "ContentSecurityPolicy": {
    "Policies": { 
      "allow_embed": {
        "Match": [ "/allow-framing/*" ],
        "Policy": {
          "frame-ancestors": [
            "-:'none'",
            "https://*.intelligentplant.com"
          ]
        }
      }
    }
  }
}
```

The CSP is enabled by default. To monitor the effects of the CSP without enforcing it, you can set it to report-only mode:

```json
{
  "ContentSecurityPolicy": {
    "ReportOnly": true,
    "Policies": { }
  }
}
```

Enabling report-only mode sets the `Content-Security-Policy-Report-Only` header in HTTP responses instead of the `Content-Security-Policy` header. See [here](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy-Report-Only) for further details.

Finally, it is also possible to register a callback that the CSP middleware will invoke for every request after the default CSP has been configured. This allows additional customisation on a case-by-case basis where required:

```csharp
app.UseContentSecurityPolicy((HttpContext context, ContentSecurityPolicyBuilder builder) => {
  // For some pages, we use images that are defined inline using data: URIs.
  if (context.Request.Path.StartsWithSegments("/needs-inline-image")) {
      var directive = builder.GetOrCreateDirective("image-src");
      directive.Add("data:");
  }
  return default;
});
```
