# Example IAS Command-Line Application

This command-line application uses a [starter template](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet) from the [Industrial App Store](https://appstore.intelligentplant.com).

Please refer to [the documentation](https://github.com/intelligentplant/IndustrialAppStore.ClientTools.DotNet/blob/main/src/IntelligentPlant.IndustrialAppStore.CommandLine/README.md) for more information about configuring authentication and calling Industrial App Store APIs.

# Getting Started

## Register as an App Developer

If you have not done so already, you can register as an Industrial App Store developer [here](https://appstore.intelligentplant.com/Developer/RegisterDeveloper).


## Create App Registration

Once you have registered as a developer, you can create a new app registration for your app. Make a note of the app ID that is generated for your app. 

Once you have created your app registration, you must enable the OAuth 2.0 device authorization flow.

Next, update the default scopes requested by your app on the app registration page. The following scopes are available:

- Personal Info (`UserInfo`) - allows your app to query the Industrial App Store for information about the authenticated user (e.g. their display name and profile picture).
- Data Read (`DataRead`) - allows your app to perform read queries on data sources and event sources that the authenticated user has granted the app access to.
- Data Write (`DataWrite`) - allows your app to perform write operations on data sources that the authenticated user has granted the app access to.

In order for this example app to function correctly, the "Personal Info" scope must be requested.


## Configure App Settings

Next, you must update the `appsettings.json` file (in the same folder as this README):

```json
{
    "IAS": {
        "ClientId": "<YOUR CLIENT ID>",
        "TokenPath": "<PATH TO SAVE AUTHENTICATION TOKENS TO>"
    }
}
```

The `TokenPath` setting specifies the path where the app will save authentication tokens to. This is required in order to persist the authentication tokens between app runs. Specifying a relative path will save the tokens relative to the user's local application data folder.


# Authentication

> Instead of requesting services directly from the root service provider, you should create a scope and request services from the scope. This is because some Industrial App Store services are scoped services and may have dependencies that are scoped services.

## Logging into the Industrial App Store

To log the app into the Industrial App Store, retrieve the `IndustrialAppStoreSessionManager` service from the scoped service provider and call the `SignInAsync` method:

```csharp
using var scope = provider.CreateScope();

var sessionManager = scope.ServiceProvider.GetRequiredService<IndustrialAppStoreSessionManager>();
await sessionManager.SignInAsync((request, ct) => {
    Console.WriteLine($"Please sign in by visiting {request.VerificationUri} and entering the following code: {request.UserCode}");
    return default;
});
```

Authentication tokens are encrypted and persisted to the `TokenPath` folder specified in the app settings (assuming that it is set). If the session manager already has a valid authentication session, the `SignInAsync` method will not prompt the user to log in again.


## Logging out of the Industrial App Store

You can remove the current authentication session by calling the `IndustrialAppStoreSessionManager.SignOutAsync` method:

```csharp
await sessionManager.SignOutAsync();
```


# Calling Data Core and Industrial App Store APIs

> Instead of requesting services directly from the root service provider, you should create a scope and request services from the scope. This is because some Industrial App Store services are scoped services and may have dependencies that are scoped services.

To call Data Core and Industrial App Store APIs, retrieve the `IndustrialAppStoreHttpClient` service from the scoped service provider:

```csharp
var client = scope.ServiceProvider.GetRequiredService<IndustrialAppStoreHttpClient>();
var userInfo = await client.UserInfo.GetUserInfoAsync();
```

The current access token for the authentication session will automatically be added to outgoing API calls.


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
