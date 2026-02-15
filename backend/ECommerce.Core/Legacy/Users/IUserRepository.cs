using ECommerce.Core.Models;

namespace ECommerce.Core.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> EmailExistsExceptAsync(string email, Guid userId);
    Task IncrementFailedLoginAttemptsAsync(Guid userId);
    Task ResetFailedLoginAttemptsAsync(Guid userId);
    Task UpdateLastLoginAsync(Guid userId);
    Task<User?> GetByPasswordResetTokenAsync(string token);
}
