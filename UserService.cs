using Ecommerce_MVC.Models;
using System.Net.Http.Json;

namespace Ecommerce_MVC.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("Api");
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User/register", model);

            return response.IsSuccessStatusCode;
        }

        public async Task<LoginResponse?> LoginAsync(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User/login", model);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }

            return null;
        }
    }
}
