using AutoMapper;
using ECommerce.Core.Misc;
using ECommerce.Core.Models;

namespace ECommerce.Core.Addresses;

public class AddressService : BaseService
{
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;

    public AddressService(IAddressRepository addressRepository, IMapper mapper)
    {
        _addressRepository = addressRepository;
        _mapper = mapper;
    }

    public async Task<AddressOutput> CreateAddressAsync(Guid userId, CreateAddressInput input)
    {
        var address = _mapper.Map<Address>(input);
        address.UserId = userId;

        if (input.IsDefault)
        {
            await ClearDefaultAddressesAsync(userId);
        }

        address = await _addressRepository.CreateAsync(address);
        return _mapper.Map<AddressOutput>(address);
    }

    public async Task<AddressOutput> UpdateAddressAsync(Guid userId, Guid addressId, UpdateAddressInput input)
    {
        var existingAddress = await _addressRepository.GetByIdAsync(addressId);
        if (existingAddress == null)
        {
            throw new InvalidOperationException("Address not found");
        }

        if (existingAddress.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this address");
        }

        _mapper.Map(input, existingAddress);
        existingAddress.UpdatedAt = DateTime.UtcNow;

        if (input.IsDefault && !existingAddress.IsDefault)
        {
            await _addressRepository.SetDefaultAsync(addressId, userId);
        }

        existingAddress = await _addressRepository.UpdateAsync(existingAddress);
        return _mapper.Map<AddressOutput>(existingAddress);
    }

    public async Task DeleteAddressAsync(Guid userId, Guid addressId)
    {
        var address = await _addressRepository.GetByIdAsync(addressId);
        if (address == null)
        {
            throw new InvalidOperationException("Address not found");
        }

        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this address");
        }

        await _addressRepository.DeleteAsync(addressId);
    }

    public async Task<List<AddressOutput>> GetUserAddressesAsync(Guid userId)
    {
        var addresses = await _addressRepository.GetUserAddressesAsync(userId);
        return _mapper.Map<List<AddressOutput>>(addresses);
    }

    public async Task<AddressOutput> SetDefaultAddressAsync(Guid userId, Guid addressId)
    {
        var address = await _addressRepository.GetByIdAsync(addressId);
        if (address == null)
        {
            throw new InvalidOperationException("Address not found");
        }

        if (address.UserId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to modify this address");
        }

        await _addressRepository.SetDefaultAsync(addressId, userId);

        var updatedAddress = await _addressRepository.GetByIdAsync(addressId);
        return _mapper.Map<AddressOutput>(updatedAddress!);
    }

    private async Task ClearDefaultAddressesAsync(Guid userId)
    {
        var addresses = await _addressRepository.GetUserAddressesAsync(userId);
        foreach (var address in addresses.Where(a => a.IsDefault))
        {
            address.IsDefault = false;
            address.UpdatedAt = DateTime.UtcNow;
            await _addressRepository.UpdateAsync(address);
        }
    }
}
