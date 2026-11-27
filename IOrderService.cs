using Ecommerce_MVC.Models;

namespace Ecommerce_MVC.Services
{
    public interface IOrderService
    {
        Task<int?> PlaceOrderAsync(
            string token,
            int addressId);

        Task<List<OrderViewModel>> GetMyOrdersAsync(
            string token);
    }
}