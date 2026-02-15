namespace ECommerce.Core.Features.User;

public interface IUserRepository
{
    Task<UserEntity?> GetByIdAsync(Guid id);
    Task<UserEntity?> GetByEmailAsync(string email);
    Task<List<UserEntity>> GetAllAsync();
    Task<UserEntity> CreateAsync(UserEntity user);
    Task<UserEntity> UpdateAsync(UserEntity user);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> UpdatePasswordAsync(Guid id, string passwordHash);
}
