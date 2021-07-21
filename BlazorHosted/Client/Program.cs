using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHosted.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

            // You can use either a named OR a typed HttpClient

            // A named HTTP client - see FetchData.razor

            builder.Services
                   .AddHttpClient("ServerApi", client => client.BaseAddress = new Uri(builder.Configuration["BaseApiAddress"]))
                   .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

            // A typed HTTP client - see WeatherForecastHttpClient.cs and FetchData2.razor

            builder.Services
                   .AddHttpClient<WeatherForecastHttpClient>(client => client.BaseAddress = new Uri(builder.Configuration["BaseApiAddress"]))
                   .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

                options.UserOptions.RoleClaim = "roles";

                options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration["UserScope"]);
            });

            builder.Services.AddApiAuthorization<RemoteAuthenticationState, RemoteUserAccount>()
                   .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, CustomUserFactory>();

            await builder.Build().RunAsync();
        }
    }
}
