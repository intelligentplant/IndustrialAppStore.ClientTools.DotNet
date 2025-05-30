﻿# Example IAS Web Application

This ASP.NET Core application uses a [starter template](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet) from the [Industrial App Store](https://appstore.intelligentplant.com).

Please refer to [the documentation](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet/blob/main/src/IntelligentPlant.IndustrialAppStore.Authentication/README.md) for more information about configuring authentication and calling Industrial App Store APIs.

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

    https://localhost:44346/auth/signin-ip

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

If you generated a secret key for your app, use the [ASP.NET Core Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) to save the secret by running the following command from the project folder:

    dotnet user-secrets set "IAS:ClientSecret" "<YOUR CLIENT SECRET>"

> Note that the Secret Manager is intended for use in development environments only; you should use a secure store such as [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/) to keep secrets safe in production environments.

__DO NOT STORE CLIENT SECRETS IN THE `appsettings.json` FILE OR IN ANY OTHER FILE THAT IS CHECKED INTO SOURCE CONTROL!__


# Default HTTP Response Headers

Your app is configured to append a set of default headers onto every HTTP response to protect against cross-site scripting attacks. The headers are defined in [appsettings.json](./appsettings.json).

Documentation about how to configure the custom headers can be found [here](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet/blob/main/src/IntelligentPlant.IndustrialAppStore.AspNetCore/README.md).


# Content Security Policy

Your app uses a [Content Security Policy](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP) (CSP) to protect against cross-site scripting attacks. The CSP is enabled by default and defined in [csp.json](./csp.json).

Documentation about how to configure the CSP can be found [here](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet/blob/main/src/IntelligentPlant.IndustrialAppStore.AspNetCore/README.md).


# Calling Data Core and Industrial App Store APIs

To call Data Core and Industrial App Store APIs from inside the HTTP request pipeline, inject the `IndustrialAppStoreHttpClient` into your controllers, for example:

```csharp
[Authorize]
public async Task<IActionResult> Index(
    [FromServices] IndustrialAppStoreHttpClient iasClient,
    CancellationToken cancellationToken = default
) {
    var dataSources = await iasClient
        .DataSources
        .GetDataSourcesAsync(cancellationToken);

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

The `IndustrialAppStoreHttpClient` uses a service called `ITokenStore` to automatically retrieve an Industrial App Store access token for the calling user and add it to outgoing requests.

> The `ITokenStore` service referenced in this section is not used to authenticate outgoing requests if your app is running in on-premises mode rather than Industrial App Store mode. See notes on creating an on-premises app below for more details.


# Debugging Your App

The project contains the following Visual Studio debugging profiles in its [launchSettings.json](./Properties/launchSettings.json) file:

- `Kestrel (IAS Mode)`
- `IIS Express (On-Premises Mode)`

The Kestrel profile is used to run and debug your application in Industrial App Store mode. It runs ASP.NET Core's Kestrel web server, using the Industrial App Store for authentication and data queries. 

The IIS Express profile is used to run and debug your application in on-premises mode. It runs IIS Express, using Windows authentication and a local Data Core API instance for data queries.

> See below for more information about writing an on-premises app.


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


# Telemetry

The app uses OpenTelemetry to collect and export telemetry data in OTLP format. More information about how to configure the export destination can be found [here](https://github.com/wazzamatazz/opentelemetry-extensions#configuring-a-multi-signal-opentelemetry-protocol-otlp-exporter). 


# Creating an On-Premises App

By default, your app is configured to use the Industrial App Store for both authentication and as the end point for Data Core API queries. However, it is also possible to write an app that is hosted on-premises, using Windows authentication and querying a local Data Core API instance instead of the Industrial App Store.

> If you want to develop an on-premises app (or an app that can be deployed both on-premises and through the Industrial App Store), please [contact Intelligent Plant](https://www.intelligentplant.com/contact-us) to request an on-premises Data Core API installation.


## Differences Between Industrial App Store and On-Premises Apps

There are some notable differences between Industrial App Store and on-premises apps. Please read the following sections carefully.


### API Availability

The Industrial App Store defines APIs for app billing, and organisation and user information queries (accessed via the `AccountTransactions`, `Organization` and `UserInfo` properties on the `IndustrialAppStoreHttpClient` class respectively). These APIs are not available when running in on-premises mode, and attempts to call these APIs will throw errors. You must account for these differences yourself.


### API Authentication and Authorization

When running in Industrial App Store mode, your app will automatically add a bearer token to outgoing requests made on behalf of the calling user, allowing the Industrial App Store to authenticate and authorize the request on a per-user basis.

When running in on-premises mode, you must ensure that the HTTP message handler used to construct the `HttpClient` for the `DataCoreHttpClient` service is configured to use an appropriate authentication scheme when making outgoing requests.
