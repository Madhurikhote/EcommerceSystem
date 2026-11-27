using Ecommerce_MVC.Models;

namespace Ecommerce_MVC.Services
{
    public interface IAddressService
    {
        Task<List<Address>?> GetAddressesAsync(string token);

        Task<bool> AddAddressAsync(
            CreateAddressModel model,
            string token);

        Task<Address?> GetAddressByIdAsync(
            int id,
            string token);

        Task<bool> UpdateAddressAsync(
            EditAddressModel model,
            string token);

        Task<bool> DeleteAddressAsync(
            int id,
            string token);
    }
}