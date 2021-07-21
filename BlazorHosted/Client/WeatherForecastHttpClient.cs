using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BlazorHosted.Shared;

using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorHosted.Client
{
    public class WeatherForecastHttpClient
    {
        private readonly HttpClient _http;

        public WeatherForecastHttpClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<WeatherForecast[]> GetForecastAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<WeatherForecast[]>(@"https://localhost:44303/WeatherForecast");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }

            return new WeatherForecast[0];
        }
    }
}
