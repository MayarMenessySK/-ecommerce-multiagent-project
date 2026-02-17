namespace ECommerce.Core.Features.User;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(DataAccessAdapter adapter) : base(adapter)
    {
    }

    public async Task<UserEntity?> GetByIdAsync(Guid id)
    {
        var query = _meta.User.Where(u => u.Id == id);
        return await ExecuteQuerySingleAsync(query);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        var query = _meta.User.Where(u => u.Email == email.ToLower());
        return await ExecuteQuerySingleAsync(query);
    }

    public async Task<List<UserEntity>> GetAllAsync()
    {
        var query = _meta.User.OrderByDescending(u => u.CreatedAt);
        return await ExecuteQueryAsync(query);
    }

    public async Task<UserEntity> CreateAsync(UserEntity user)
    {
        user.Id = Guid.NewGuid();
        user.Email = user.Email.ToLower();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        await SaveAsync(user);
        return user;
    }

    public async Task<UserEntity> UpdateAsync(UserEntity user)
    {
        user.Email = user.Email.ToLower();
        user.UpdatedAt = DateTime.UtcNow;
        await SaveAsync(user);
        return user;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user == null) return false;
        
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        return await SaveAsync(user);
    }

    public async Task<bool> UpdatePasswordAsync(Guid id, string passwordHash)
    {
        var user = await GetByIdAsync(id);
        if (user == null) return false;
        
        user.PasswordHash = passwordHash;
        user.UpdatedAt = DateTime.UtcNow;
        return await SaveAsync(user);
    }
}
