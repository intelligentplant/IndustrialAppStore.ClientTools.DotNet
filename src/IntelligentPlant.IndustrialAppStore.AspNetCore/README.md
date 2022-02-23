# IntelligentPlant.IndustrialAppStore.AspNetCore

This project defines services and middleware for use in ASP.NET Core applications.


# Installation

Install the [IntelligentPlant.IndustrialAppStore.AspNetCore](https://www.nuget.org/packages/IntelligentPlant.IndustrialAppStore.AspNetCore) package by following the instructions on NuGet.org.


# Custom Headers

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


# Content Security Policy

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

The CSP configuration is mapped to the [ContentSecurityPolicyOptions](./ContentSecurityPolicyOptions.cs) class at runtime. Each value in the `Policies` section is mapped to a [ContentSecurityPolicyDefinition](./ContentSecurityPolicyDefinition.cs) object, with the key being an identifier for the policy.

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
  return Task.CompletedTask;
});
```
