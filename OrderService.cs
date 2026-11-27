using Ecommerce_MVC.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Ecommerce_MVC.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        // =========================
        // Place Order
        // =========================

        public async Task<int?> PlaceOrderAsync(
            string token,
            int addressId)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            var response = await _httpClient.PostAsync(
                $"api/Order/place?addressId={addressId}",
                null);

            if (!response.IsSuccessStatusCode)
                return null;

            var json =
                await response.Content.ReadAsStringAsync();

            using var document =
                JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty(
                "orderId",
                out var orderId))
            {
                return orderId.GetInt32();
            }

            return null;
        }


        // =========================
        // Get My Orders
        // =========================

        public async Task<List<OrderViewModel>>
            GetMyOrdersAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            var response =
                await _httpClient.GetAsync(
                    "api/Order/my-orders");

            if (!response.IsSuccessStatusCode)
                return new List<OrderViewModel>();

            var json =
                await response.Content.ReadAsStringAsync();

            var orders =
                JsonSerializer.Deserialize<
                    List<OrderViewModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return orders ??
                new List<OrderViewModel>();
        }
    }
}