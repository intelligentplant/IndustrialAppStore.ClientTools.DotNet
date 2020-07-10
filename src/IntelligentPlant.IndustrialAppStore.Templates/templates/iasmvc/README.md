# Example IAS Application

This ASP.NET Core application uses a [starter template](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet) from the [Industrial App Store](https://appstore.intelligentplant.com).


# Getting Started

If you have not done so already, you can register as an Industrial App Store developer [here](https://appstore.intelligentplant.com/Developer/RegisterDeveloper).

Once you have registered as a developer, you can create a new app registration for your app. Make a note of the app ID that is generated for your app. You will also need to add the following redirect URL to the app registration:

    https://localhost:44300/auth/signin-ip

Once you have created your app registration, you should generate a secret key for your app. Your app authenticates users via the OAuth 2.0 authorization code flow, using the [Proof Key for Code Exchange (PKCE)](https://oauth.net/2/pkce/) extension. PKCE is an extension to the OAuth2 authorization code flow to enable a more secure transaction when exchanging an authorization code for an access token. It is possible for your app to authenticate users without requiring a secret key. However, for maximum security, you should *always* generate a secret key unless your app requires you to publicly distribute an executable file.

Note that you may need to enable the use of PKCE on the app registration page; in a future release, PKCE will be enabled for all apps by default.

Next, update the default scopes requested by your app on the app registration page. The following scopes are available:

- Personal Info (`UserInfo`) - allows your app to query the Industrial App Store for information about the authenticated user (e.g. their display name and profile picture).
- Data Read (`DataRead`) - allows your app to perform read queries on data sources and event sources that the authenticated user has granted the app access to.
- Data Write (`DataWrite`) - allows your app to perform write operations on data sources that the authenticated user has granted the app access to.
- Account Transactions (`AccountDebit`) - allows your app to perform billing operations.

In order for this app to function correctly, both "Personal Info" and "Data Read" scopes must be requested.

Next, you must update the `appsettings.json` file (in the same folder as this README):

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>"
    }
}
```


## Client Secrets

If you generated a secret key for your app, use the [ASP.NET Core Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) to save the secret using the following command:

    dotnet user-secrets set "IAS:ClientSecret" "<YOUR CLIENT SECRET>"

Note that the Secret Manager is intended for use in development environments only; you should use a secure store such as [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/) to keep secrets safe in production environments.

Alternatively, you can specify the client secret directly in the `appsettings.json` file:

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>",
        "ClientSecret": "<YOUR CLIENT SECRET>"
    }
}
```

__NOTE THAT IT IS STRONGLY RECOMMENDED THAT YOU DO NOT STORE CLIENT SECRETS IN THIS WAY!__


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
