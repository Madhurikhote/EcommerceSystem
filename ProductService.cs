using Ecommerce_MVC.Models;
using System.Net.Http.Json;

namespace Ecommerce_MVC.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("Api");
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _httpClient.GetFromJsonAsync<List<Product>>(
                "api/Product");

            return products ?? new List<Product>();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Product>(
                $"api/Product/{id}");
        }

        public async Task<SearchResult?> SearchProductsAsync(
            ProductSearch search)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(search.Keyword))
                queryParams.Add(
                    $"Keyword={Uri.EscapeDataString(search.Keyword)}");

            if (!string.IsNullOrWhiteSpace(search.Category))
                queryParams.Add(
                    $"Category={Uri.EscapeDataString(search.Category)}");

            if (!string.IsNullOrWhiteSpace(search.Brand))
                queryParams.Add(
                    $"Brand={Uri.EscapeDataString(search.Brand)}");

            if (search.MinPrice.HasValue)
                queryParams.Add($"MinPrice={search.MinPrice.Value}");

            if (search.MaxPrice.HasValue)
                queryParams.Add($"MaxPrice={search.MaxPrice.Value}");

            if (!string.IsNullOrWhiteSpace(search.Sort))
                queryParams.Add(
                    $"Sort={Uri.EscapeDataString(search.Sort)}");

            queryParams.Add($"Page={search.Page}");
            queryParams.Add($"PageSize={search.PageSize}");

            string url = "api/Product/search";

            if (queryParams.Count > 0)
            {
                url += "?" + string.Join("&", queryParams);
            }

            return await _httpClient.GetFromJsonAsync<SearchResult>(url);
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var categories =
                await _httpClient.GetFromJsonAsync<List<string>>(
                    "api/Product/categories");

            return categories ?? new List<string>();
        }
    }
}