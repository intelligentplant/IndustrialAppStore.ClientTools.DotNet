# Industrial App Store Operations

In addition to the standard Data Core API operations, the Industrial App Store offers APIs for performing IAS-specific operations, such as requesting information about IAS users and their organisations, and billing users for app usage.

Please note that the API calls described in this page are only available when using the [IntelligentPlant.IndustrialAppStore.Authentication](/src/IntelligentPlant.IndustrialAppStore.Authentication) and [IntelligentPlant.IndustrialAppStore.HttpClient](/src/IntelligentPlant.IndustrialAppStore.HttpClient) libraries to query the Industrial App Store.

_The `IntelligentPlant.IndustrialAppStore.Client` namespace contains extension methods to simplify some IAS API calls._


## Requesting Information About the Calling User

> **NOTE:**
> IAS apps can only perform this action if they have been granted the `UserInfo` scope.


User information is retrieved by calling the `GetUserInfoAsync` method on the API client's `UserInfo` property:

```csharp
var userInfo = await client.UserInfo.GetUserInfoAsync(Request.HttpContext, cancellationToken);
```

The result of the call is a [UserOrGroupPrincipal](/src/IntelligentPlant.IndustrialAppStore.HttpClient/Model/UserOrGroupPrincipal.cs) object describing the calling user.


## Organisation Queries

If the authenticated user is a member of an organisation, you can perform API calls against the user's organisation.


### User Search

> **NOTE:**
> IAS apps can only perform this action if they have been granted the `UserInfo` scope.


Your app may need to search for users in the caller's organisation (for example, if you write a charting app and you want users to be able to share charts that they have created).

You can search for users within the caller's organisation using the `FindUsersAsync` method on the client's `Organization` property:

```csharp
// Finds users that match the specified filter (extension method).
var users = await client.Organization.FindUsersAsync(
    "Smith",
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Finds users using a UserOrGroupPrincipalSearchRequest object.
users = await client.Organization.FindUsersAsync(
    new UserOrGroupPrincipalSearchRequest() {
        Filter = "Smith",
        Page = 1,
        PageSize = 10,
        // Set to true to include matching users from trusted 3rd party organisations.
        IncludeExternalResults = false
    },
    Request.HttpContext,
    cancellationToken
);
```

The result of the call is a collection of [UserOrGroupPrincipal](/src/IntelligentPlant.IndustrialAppStore.HttpClient/Model/UserOrGroupPrincipal.cs) objects describing the matching users.


### Group Search

> **NOTE:**
> IAS apps can only perform this action if they have been granted the `UserInfo` scope.


Organisations in the Industrial App Store can create user groups to control access to apps, data sources, and so on. Your app can search organisation groups if required (e.g. if you want to allow a user to share something with a group rather than with individual users).

You can search for users within the caller's organisation using the `FindGroupsAsync` method on the client's `Organization` property:

```csharp
// Finds users that match the specified filter (extension method).
var users = await client.Organization.FindGroupsAsync(
    "Admin",
    context: Request.HttpContext,
    cancellationToken: cancellationToken
);

// Finds users using a UserOrGroupPrincipalSearchRequest object.
users = await client.Organization.FindGroupsAsync(
    new UserOrGroupPrincipalSearchRequest() {
        Filter = "Admin",
        Page = 1,
        PageSize = 10,
        // Set to true to include matching groups from trusted 3rd party organisations.
        IncludeExternalResults = false
    },
    Request.HttpContext,
    cancellationToken
);
```

The result of the call is a collection of [UserOrGroupPrincipal](/src/IntelligentPlant.IndustrialAppStore.HttpClient/Model/UserOrGroupPrincipal.cs) objects describing the matching groups.


### Requesting Group Memberships

> **NOTE:**
> IAS apps can only perform this action if they have been granted the `UserInfo` scope.


In order to manage access to resources in your app, you can request information about the Industrial App Store user groups that an organisation user belongs to.

You can request the calling user's group memberships by calling the `GetGroupMembershipsAsync` method on the client's `Organization` property:

```csharp
var groupMemberships = await client.Organization.GetGroupMembershipsAsync(Request.HttpContext, cancellationToken);
```

The result of the call is a collection of [UserOrGroupPrincipal](/src/IntelligentPlant.IndustrialAppStore.HttpClient/Model/UserOrGroupPrincipal.cs) objects describing the matching groups.
