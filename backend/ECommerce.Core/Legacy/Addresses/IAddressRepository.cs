using ECommerce.Core.Models;

namespace ECommerce.Core.Addresses;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id);
    Task<List<Address>> GetUserAddressesAsync(Guid userId);
    Task<Address> CreateAsync(Address address);
    Task<Address> UpdateAsync(Address address);
    Task DeleteAsync(Guid id);
    Task SetDefaultAsync(Guid addressId, Guid userId);
}
