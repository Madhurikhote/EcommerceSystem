using Ecommerce_MVC.Models;

namespace Ecommerce_MVC.Services
{
    public interface IWishlistService
    {
        Task<bool> AddToWishlistAsync(
            CreateWishlistViewModel model,
            string token);

        Task<List<WishlistViewModel>> GetWishlistAsync(
            string token);

        Task<bool> RemoveFromWishlistAsync(
            int id,
            string token);
    }
}
