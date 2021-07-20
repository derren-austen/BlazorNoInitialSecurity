using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorHosted.Client
{
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        private static readonly string scope = @"api://a7058ec1-1b3a-4ef2-a94b-f3a6d6512858/access_as_user";

        public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager) : base(provider, navigationManager)
            => ConfigureHandler(authorizedUrls: new[] { "https://localhost:44303" }, scopes: new[] { scope });
    }
}
