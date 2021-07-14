# Add Azure AD authentication to a hosted Blazor WebAssembly application

This document will outline how to take a hosted WebAssembly app, with no authentication, and secure it with Azure AD.

## Add dev config settings

Run the `add-dev-config-settings.ps1` script in the solution root.

This adds `appsettings.Development.json` files to the API app, Blazor server host app and Blazor client app.

## Wire up the client to the API app

The `add-dev-config-settings.ps1` script run in the previous step will have added an `appsettings.Development.json` file to the `wwwroot` folder in the client app.

Let's add a `BaseApiAddress` setting to both files.  The `appsettings.json` file is used in production so for now just add a place holder value.

Example:
```json
{
    "BaseApiAddress": "https://localhost:44303"
}
```

The `BaseApiAddress` in the example above is the local version of the API app.  Change the port number accordingly.

> Top tip.  If you like to use Kestrel for local development, with its default port of 5001, then use IIS to host the API app with it's auto-generated port number.

Now we need to amend the `Program.cs` file in the client app to use this setting.

Change the `BaseAddress` of the scoped `HttpClient` service.

Before:
```c#
BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
```

After:
```c#
BaseAddress = new Uri(builder.Configuration["BaseApiAddress"])
```

## Add a CORS policy to the API app

Now that the client is calling an API on a different origin we need to configure CORS in the API app.

Add an `AllowedOrigins` setting to `appsettings.json` and `appsettings.Development.json`.

```json
"AllowedOrigins": "https://localhost:5001"
```

The value in `appsettings.json` can be a place holder as it is only used in production.

Add the following to the `Configure` method in `Startup.cs` between `app.UseRouting();` and `app.UseAuthentication();`

```c#
app.UseCors(policy =>
{
    policy.WithOrigins(Configuration["AllowedOrigins"])
          .AllowAnyMethod()
          .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization)
          .AllowCredentials();
});
```

## Add app registrations to your Azure AD tenant

> Azure Active Directory (Azure AD) is Microsoft’s cloud-based identity and access management service, which helps your employees sign in and access resources. An Azure AD tenant represents your organisation and helps you to manage a specific instance of Microsoft cloud services for your internal and external users.

An app registration is required for the client and API app.

Let's head over to the Azure [portal](https://portal.azure.com).

Search for Azure Active Directory.  Click on the service.

![search for aad](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/search-aad.png)

Under Manage, click on App registrations.

![app registrations](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/app-registrations.png)

Let's add an app registration for the **client app**.

![new registration](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/new-registration.png)

Add a name.  
Leave the Supported account types as Single tenant.  
Make sure the client type is Single-page application (SPA).  
Add a redirect URI.  Use `https://localhost:5001/authentication/login-callback`.

![add registration](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/add-registration.png)

Select Register to create the app registration.

On successful registration you'll be taken to the new app.

Under Manage click on Authentication.

Make sure the two checkboxes under Implicit grant and hybrid flows are **NOT** checked.

![implicit and hybrid not checked](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/implicit-hybrid-not-checked.png)

Under Manage click on API permissions.

![client api permissions](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/client-manage-perms.png)

Click on Grant admin consent for Default Directory.

![grant admin consent](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/grant-admin-consent.png)

> Default Directory in these example is just the name of the tenant.  Yours will most likely have a different name.

The default `User.Read` permission, or scope, will then automatically be consented to.

> Scope is a mechanism in OAuth 2.0 to limit an application's access to a user's account. An application can request one or more scopes, this information is then presented to the user in the consent screen, and the access token issued to the application will be limited to the scopes granted.

Click Yes under Grant admin consent confirmation.

![yes to consent](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/consent-yes.png)

Notice the Status for this permission has changed to Granted for Default Directory.

![consent granted](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/consent-granted.png)

We shall return to the client app registration later!

Now let's add an app registration for the **API app**.

Under Manage, click on App registrations.

Add a name.  
Leave the Supported account types as Single tenant.  
As this is an API app there are no authentication flows so just fill in these details.

![add api registration](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/add-api-registration.png)

This App registration exposes an API and defines roles for the API project authorization. An **access_as_user** scope is added to the Azure App registration which is a delegated scope type.

Under Manage, click on Expose an API.

![expose an api](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/expose-api.png)

Click on Add a scope.

![add a scope](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/add-scope.png)

Click Save and continue to accept the auto-generated Application ID URI.

Populate the Edit a scope section as below.

![edit scope](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/edit-scope.png)

With the API app now in-place let's head back to the client app registration.

We need to update the client app registration to allow it to request the appropriate scopes when users login.

Under Manage, click on API permissions.

Click on Add a permission.

![add a permission](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/add-permission.png)

Select My APIs.  
Find the API we just created, click on it and select the `access_as_a_user` permission.

![request permissions](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/client-request-perms.png)

Click on the Grant admin consent for Default Directory once again.  
The will ensure the `access_as_a_user` scope is granted accordingly.

![access as a user granted permission](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/access-as-user-granted-perm.png)

## Add application roles

Application roles need to be added to the client and API applications.


Under Manage, click on App roles.

![app roles](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/manage-app-roles.png)

Click on Create app role.

![create app role](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/create-app-role.png)

Populate the Create app role section.

![](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/create-app-role-details.png)

Once the appropriate roles have been created in the client application the identical roles also need to be created in the API application.

The easiest way to do this is to copy the `appRoles` property from the manifest.

Under Manage, click on Manifest.

![manifest](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/manifest.png)

Copy `appRoles` and replace the same section in the API application with this version.

![app roles in manifest](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/app-roles-manifest.png)

This section in the API application's manifest will be empty.

![empty app roles in manifest](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/app-roles-empty.png)

## Add application roles to users

From the Active Directory blade, under Manage, click Enterprise applications.

![manage enterprise applications](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/manage-enterprise-apps.png)

**You'll need to add users to the appropriate roles in the client and API apps.**

Click on the client app registration.

Under Manage, click on Users and groups.

![users and groups](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/users-and-groups.png)

Now click on Add user/group.

![add a user](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/add-user-group.png)

From the Add Assignment page select a user and a role.

Before:  
![add roles before](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/add-roles-before.png)

After:  
![add roles after](https://raw.githubusercontent.com/derren-austen/BlazorNoInitialSecurity/main/Images/add-roles-after.png)

**This process has to be repeated if multiple roles need to be added to a user**

