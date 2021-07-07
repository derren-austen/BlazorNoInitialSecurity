# Blazor WebAssembly with no authentication

This document will outline how to take a hosted WebAssembly app, with no authentication, and secure it with Azure AD.

### Add dev config settings

Run the `add-dev-config-settings.ps1` script in the solution root.

This adds `appsettings.Development.json` files to the API app, Blazor server host app and Blazor client app.

