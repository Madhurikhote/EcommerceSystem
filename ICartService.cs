using Ecommerce_MVC.Models;

namespace Ecommerce_MVC.Services
{
    public interface ICartService
    {
        Task<bool> AddToCartAsync(AddToCartModel model, string token);

        Task<Cart?> GetCartAsync(string token);

        Task<bool> UpdateQuantityAsync(int cartItemId, int quantity, string token);

        Task<bool> RemoveItemAsync(int id, string token);
    }
}
