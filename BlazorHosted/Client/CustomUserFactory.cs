using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace BlazorHosted.Client
{
    public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        public CustomUserFactory(NavigationManager navigationManager, IAccessTokenProviderAccessor accessor) : base(accessor)
        {
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var initialUser = await base.CreateUserAsync(account, options);

            if (initialUser.Identity.IsAuthenticated)
            {
                if (account.AdditionalProperties.ContainsKey("roles"))
                {
                    var roles = account.AdditionalProperties["roles"] as JsonElement?;

                    if (roles?.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement element in roles.Value.EnumerateArray())
                        {
                            ((ClaimsIdentity)initialUser.Identity).AddClaim(new Claim("roles", element.GetString()));
                        }
                    }
                }

                if (account.AdditionalProperties.ContainsKey("groups"))
                {
                    var roles = account.AdditionalProperties["groups"] as JsonElement?;

                    if (roles?.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement element in roles.Value.EnumerateArray())
                        {
                            ((ClaimsIdentity)initialUser.Identity).AddClaim(new Claim("groups", element.GetString()));
                        }
                    }
                }
            }

            return initialUser;
        }
    }
}