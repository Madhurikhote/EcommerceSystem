using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ecommerce_MVC.Models;

namespace Ecommerce_MVC.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;

        public CartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> AddToCartAsync(AddToCartModel model, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync(
                $"api/Cart/add?productId={model.ProductId}&quantity={model.Quantity}",
                null);

            return response.IsSuccessStatusCode;
        }

        public async Task<Cart?> GetCartAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/Cart");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Cart>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public async Task<bool> UpdateQuantityAsync(int cartItemId, int quantity, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsync(
                $"api/Cart/update?cartItemId={cartItemId}&quantity={quantity}",
                null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveItemAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/Cart/remove/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}