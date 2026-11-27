using Ecommerce_MVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Ecommerce_MVC.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly HttpClient _httpClient;

        public WishlistService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Add Wishlist
        public async Task<bool> AddToWishlistAsync(
            CreateWishlistViewModel model,
            string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "api/Wishlist",
                content);

            return response.IsSuccessStatusCode;
        }

        // Get Wishlist
        public async Task<List<WishlistViewModel>> GetWishlistAsync(
            string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(
                "api/Wishlist");

            if (!response.IsSuccessStatusCode)
                return new List<WishlistViewModel>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<WishlistViewModel>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
                ?? new List<WishlistViewModel>();
        }

        // Remove Wishlist
        public async Task<bool> RemoveFromWishlistAsync(
            int id,
            string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync(
                $"api/Wishlist/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
