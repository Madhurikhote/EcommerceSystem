using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ecommerce_MVC.Models;

namespace Ecommerce_MVC.Services
{
    public class AddressService : IAddressService
    {
        private readonly HttpClient _httpClient;

        public AddressService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get all addresses
        public async Task<List<Address>?> GetAddressesAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/Address");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Address>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        // Add address
        public async Task<bool> AddAddressAsync(
            CreateAddressModel model,
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
                "api/Address",
                content);

            return response.IsSuccessStatusCode;
        }

        // Delete address
        public async Task<bool> DeleteAddressAsync(
            int id,
            string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync(
                $"api/Address/{id}");

            return response.IsSuccessStatusCode;
        }
        // Get address by ID
        public async Task<Address?> GetAddressByIdAsync(
            int id,
            string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(
                $"api/Address/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Address>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        // Update address
        public async Task<bool> UpdateAddressAsync(
            EditAddressModel model,
            string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync(
                $"api/Address/{model.Id}",
                content);

            return response.IsSuccessStatusCode;
        }
    }
}
