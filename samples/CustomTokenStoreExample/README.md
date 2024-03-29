﻿# CustomTokenStoreExample

This project demonstrates how to use a custom [ITokenStore](../../src/IntelligentPlant.IndustrialAppStore.Authentication/ITokenStore.cs) to store access and refresh tokens obtained by an [Industrial App Store](https://appstore.intelligentplant.com) app.

The functionality of the app itself is based on the example MVC application found [here](../ExampleMvcApplication).

The [EFTokenStore](./Services/EFTokenStore.cs) class uses an Entity Framework Core database context to load and save tokens. Note that the project is configured to use an in-memory SQLite database that will not persist any data between app restarts!

Tokens are protected at rest using the `ProtectedData` class. This class is not available on non-Windows platforms; an alternative encryption solution would be required if a different platform was being targeted, or if a common database was being shared by multiple app instances.

The project also demonstrates how the [BackchannelIndustrialAppStoreHttpClient](../../src/IntelligentPlant.IndustrialAppStore.Authentication/BackchannelIndustrialAppStoreHttpClient.cs) can be used to perform backchannel API calls to the Industrial App Store outside of the app's HTTP request pipeline. The [BackgroundGroupFetcher](./Services/BackgroundGroupFetcher.cs) class is an `IHostedService` implementation that is notified when a user signs into the app and downloads information about the user's Industrial App Store group memberships.


# Bootstrapping

To run this example, you must first [register as an app developer](https://appstore.intelligentplant.com/Developer/RegisterDeveloper) on the Industrial App Store and then add an app registration. Once you have created the registration, add the following redirect URL:

    https://localhost:44346/auth/signin-ip

Next, copy the client ID for your app registration and paste it into the `IAS:ClientId` setting in [appsettings.json](./appsettings.json) e.g.

```json
{
  "IAS": {
    "ClientId": "ABCDEF0123456789"
  }
}
```
