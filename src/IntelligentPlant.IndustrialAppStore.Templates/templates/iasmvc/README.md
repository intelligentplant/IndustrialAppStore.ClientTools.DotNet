# Example IAS Application

This ASP.NET Core application uses a [starter template](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet) from the [Industrial App Store](https://appstore.intelligentplant.com).

The application uses the following libraries for client-side functionality:

- [Bootstrap](https://getbootstrap.com/)
- [ChartJS](https://www.chartjs.org/)
- [FontAwesome](https://fontawesome.com/)
- [jQuery](https://jquery.com/)
- [RxJS](https://rxjs.dev/)

Please refer to the documentation for the above libraries for queries about their usage.

Client libraries are managed using [LibMan](https://docs.microsoft.com/en-us/aspnet/core/client-side/libman/libman-vs) and are managed through the `libman.json` file.


# Getting Started

## Register as an App Developer

If you have not done so already, you can register as an Industrial App Store developer [here](https://appstore.intelligentplant.com/Developer/RegisterDeveloper).


## Create App Registration

Once you have registered as a developer, you can create a new app registration for your app. Make a note of the app ID that is generated for your app. You will also need to add the following redirect URL to the app registration:

    https://localhost:44300/auth/signin-ip

Once you have created your app registration, you should generate a secret key for your app. Your app authenticates users via the OAuth 2.0 authorization code flow, using the [Proof Key for Code Exchange (PKCE)](https://oauth.net/2/pkce/) extension. PKCE is an extension to the OAuth2 authorization code flow to enable a more secure transaction when exchanging an authorization code for an access token. It is possible for your app to authenticate users without requiring a secret key. However, for maximum security, you should *always* generate a secret key unless your app requires you to publicly distribute an executable file.

Next, update the default scopes requested by your app on the app registration page. The following scopes are available:

- Personal Info (`UserInfo`) - allows your app to query the Industrial App Store for information about the authenticated user (e.g. their display name and profile picture).
- Data Read (`DataRead`) - allows your app to perform read queries on data sources and event sources that the authenticated user has granted the app access to.
- Data Write (`DataWrite`) - allows your app to perform write operations on data sources that the authenticated user has granted the app access to.
- Account Transactions (`AccountDebit`) - allows your app to perform billing operations.

In order for this app to function correctly, both "Personal Info" and "Data Read" scopes must be requested.


## Configure App Settings

Next, you must update the `appsettings.json` file (in the same folder as this README):

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>"
    }
}
```


### Client Secrets

If you generated a secret key for your app, use the [ASP.NET Core Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) to save the secret using the following command:

    dotnet user-secrets set "IAS:ClientSecret" "<YOUR CLIENT SECRET>"

> Note that the Secret Manager is intended for use in development environments only; you should use a secure store such as [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/) to keep secrets safe in production environments.

Alternatively, you can specify the client secret directly in the `appsettings.json` file:

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>",
        "ClientSecret": "<YOUR CLIENT SECRET>"
    }
}
```

__NOTE THAT IT IS STRONGLY RECOMMENDED THAT YOU DO NOT STORE CLIENT SECRETS IN THE `appsettings.json` FILE!__


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

# Preparing Your App For Publication

The project contains a Visual Studio [publish profile](./Properties/PublishProfiles/IndustrialAppStoreAuth.pubxml) that can be used to build your app using the Release configuration and create a set of files that can be copied to your deployment destination.

See [here](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/visual-studio-publish-profiles#publish-profiles) for more information about publish profiles.


# Creating an On-Premises App

By default, your app is configured to use the Industrial App Store for both authentication and as the end point for Data Core API queries. However, it is also possible to write an app that is hosted on-premises, using Windows authentication and querying a local Data Core API instance instead of the Industrial App Store.

> If you want to develop an on-premises app (or an app that can be accessed both on-premises and through the Industrial App Store), please [contact Intelligent Plant](https://www.intelligentplant.com/contact-us) to request an on-premises Data Core API installation.


## Debugging an On-Premises App

The Visual Studio solution for your app contains two debugging profiles:

- Kestrel (IAS Authentication)
- IIS Express (Windows Authentication)

The Kestrel profile is used to run and debug your application using ASP.NET Core's Kestrel web server, using the Industrial App Store for authentication and data queries, whereas the IIS Express profile is used to run and debug your application using IIS Express, using Windows authentication and a local Data Core API instance for data queries.


## Differences Between Industrial App Store and On-Premises Apps

### API Availability

The Industrial App Store defines APIs for app billing, organisation, and user information queries (accessed via the `AccountTransactions`, `Organization` and `UserInfo` properties on the `IndustrialAppStoreHttpClient` class respectively). These APIs are not available when running in on-premises mode, and attempts to call these APIs will throw errors. You must account for these differences yourself.

You can detect if your app is running in Industrial App Store or on-premises mode by injecting the `IndustrialAppStoreAuthenticationOptions` service into your classes and getting the value of its `UseExternalAuthentication` property. The `UseExternalAuthentication` property will be `true` when your app is running in on-premises mode.

> See the `Controllers/AccountController.cs` file in this project for an example of how to perform the check as described above.


### API Authentication

When running in Industrial App Store mode, your app will automatically add a bearer token to outgoing requests made on behalf of the calling user, allowing the Industrial App Store to authenticate and authorize the request on a per-user basis. When running in on-premises mode, the default network credentials (i.e. [CredentialCache.DefaultNetworkCredentials](https://docs.microsoft.com/en-us/dotnet/api/system.net.credentialcache.defaultnetworkcredentials)) are added to outgoing HTTP requests instead.

> Note that the differences in API authentication mean that it may not be possible to perform per-user authentication when calling the on-premises Data Core API. You should be prepared to apply your own authorization scheme when running in on-premises mode in order to restrict access to the data sources configured in the Data Core API where appropriate.


### Publish Profile for On-Premises Deployment

In addition to the [Industrial App Store publish profile](./Properties/PublishProfiles/IndustrialAppStoreAuth.pubxml) described above, the project also contains an [on-premises publish profile](./Properties/PublishProfiles/IISWindowsAuth.pubxml) that can be used to prepare your app for publishing to an on-premises instance of IIS.
