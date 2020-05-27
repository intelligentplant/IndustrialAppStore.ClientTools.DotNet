# Example IAS Application

This ASP.NET Core application uses the [Industrial App Store](https://appstore.intelligentplant.com) starter template.


# Getting Started

If you have not done so already, you can register as an Industrial App Store developer [here](https://appstore.intelligentplant.com/Developer/RegisterDeveloper).

Once you have registered, you can create a new app registration for your app. Make a note of the app ID that is generated for your app. Once you have created the app registration, you will need to add the following redirect URL to the registration:

    https://localhost:44300/auth/signin-ip

Next, you can do one of the following:

1. Generate a secret key for your app.
2. Enable Proof Key for Code Exchange (PKCE) for your app.

[PKCE](https://oauth.net/2/pkce/) is an extension to the OAuth2 authorization code flow that allows you to sign users into your app without requiring a secret key.

Next, update the default scopes requested by your app. The following scopes are available:

- Personal Info (`UserInfo`) - allows your app to query the Industrial App Store for information about the authenticated user (e.g. their display name and profile picture).
- Data Read (`DataRead`) - allows your app to perform read queries on data sources and event sources that the authenticated user has granted the app access to.
- Data Write (`DataWrite`) - allows your app to perform write operations on data sources that the authenticated user has granted the app access to.
- Account Transactions (`AccountDebit`) - allows your app to perform billing operations.

In order for this app to function correctly, both "Personal Info" and "Data Read" scopes must be requested.

Next, you must update the `appsettings.json` file for your app. If you generated a secret key for your app, update the file as follows:

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>",
        "ClientSecret": "<YOUR CLIENT SECRET>"
    }
}
```

Alternatively, if you enabled PKCE for your app, update the file as follows:

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>",
        "UsePkce": true
    }
}
```

# Calling IAS APIs

To call Industrial App Store APIs, inject the `IndustrialAppStoreHttpClient` into your controllers, for example:

```csharp
[Authorize]
public async Task<IActionResult> Index(
    [FromServices] IndustrialAppStoreHttpClient iasClient,
    CancellationToken cancellationToken = default
) {
    var dataSources = await iasClient.DataSources.GetDataSourcesAsync(
        Request.HttpContext,
        cancellationToken
    );

    var selectItems = new List<SelectListItem>() { 
        new SelectListItem() {
            Text = "Select a data source",
            Disabled = true,
            Selected = true
        }
    };

    selectItems.AddRange(dataSources.Select(x => new SelectListItem() { 
        Text = x.Name.DisplayName, 
        Value = x.Name.QualifiedName 
    }));

    var viewModel = new IndexViewModel() {
        DataSources = selectItems
    };
    return View(viewModel);
}
```

The `IndustrialAppStoreHttpClient` uses a service called `ITokenStore` to automatically retrieve the Industrial App Store access token from the calling user's `HttpContext` and add it to outgoing requests made on behalf of the calling user.


# Advanced Configuration

## Scopes

Specifying default scopes on the app's registration page simplifies the configuration of your app in code. However, it is also possible to specify scopes in the `appsettings.json` file instead of (or in addition to) using the default scopes in the app registration:

```json
{
    "IAS": {
        "Scope": [
            "UserInfo",
            "DataRead"
        ]
    }
}
```

## Refresh Tokens

If your app requires you to perform background operations even when the user is not actively using your app, or if you want to keep the user signed into your app perpetually, you can request a refresh token when you sign a user in. Refresh tokens can be enabled in one of two ways:

1. By enabling refresh tokens by default in the app's Industrial App Store registration page.
2. By modifying the `appsettings.json` configuration file for the app.

If you choose the second option, the `appsettings.json` file is updated as follows:

```json
{
    "IAS": {
        "RequestRefreshToken": true
    }
}
```
