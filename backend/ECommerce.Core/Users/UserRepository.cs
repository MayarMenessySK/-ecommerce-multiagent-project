using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using System.Data;

namespace ECommerce.Core.Users;

public class UserRepository : BaseRepository, IUserRepository
{
    public UserRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM users WHERE id = @Id";
        return await QueryFirstOrDefaultAsync(sql, MapUser, new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var sql = "SELECT * FROM users WHERE LOWER(email) = LOWER(@Email)";
        return await QueryFirstOrDefaultAsync(sql, MapUser, new { Email = email });
    }

    public async Task<User> CreateAsync(User user)
    {
        var sql = @"
            INSERT INTO users (id, email, password_hash, first_name, last_name, phone_number, role, 
                is_active, is_email_verified, created_at, updated_at)
            VALUES (@Id, @Email, @PasswordHash, @FirstName, @LastName, @PhoneNumber, @Role, 
                @IsActive, @IsEmailVerified, @CreatedAt, @UpdatedAt)
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapUser, new
        {
            user.Id,
            user.Email,
            user.PasswordHash,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Role,
            user.IsActive,
            user.IsEmailVerified,
            user.CreatedAt,
            user.UpdatedAt
        }) ?? throw new Exception("Failed to create user");
    }

    public async Task<User> UpdateAsync(User user)
    {
        var sql = @"
            UPDATE users 
            SET first_name = @FirstName, last_name = @LastName, phone_number = @PhoneNumber,
                password_hash = @PasswordHash, password_reset_token = @PasswordResetToken,
                password_reset_expires = @PasswordResetExpires, email_verification_token = @EmailVerificationToken,
                is_email_verified = @IsEmailVerified, is_active = @IsActive, 
                failed_login_attempts = @FailedLoginAttempts, lockout_end = @LockoutEnd,
                last_login_at = @LastLoginAt, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapUser, new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.PasswordHash,
            user.PasswordResetToken,
            user.PasswordResetExpires,
            user.EmailVerificationToken,
            user.IsEmailVerified,
            user.IsActive,
            user.FailedLoginAttempts,
            user.LockoutEnd,
            user.LastLoginAt,
            user.UpdatedAt
        }) ?? throw new Exception("Failed to update user");
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM users WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var sql = "SELECT COUNT(*) FROM users WHERE LOWER(email) = LOWER(@Email)";
        var count = await ExecuteScalarAsync<int>(sql, new { Email = email });
        return count > 0;
    }

    public async Task<bool> EmailExistsExceptAsync(string email, Guid userId)
    {
        var sql = "SELECT COUNT(*) FROM users WHERE LOWER(email) = LOWER(@Email) AND id != @UserId";
        var count = await ExecuteScalarAsync<int>(sql, new { Email = email, UserId = userId });
        return count > 0;
    }

    public async Task IncrementFailedLoginAttemptsAsync(Guid userId)
    {
        var sql = @"
            UPDATE users 
            SET failed_login_attempts = failed_login_attempts + 1,
                lockout_end = CASE WHEN failed_login_attempts >= 4 THEN CURRENT_TIMESTAMP + INTERVAL '15 minutes' ELSE lockout_end END
            WHERE id = @UserId";
        await ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task ResetFailedLoginAttemptsAsync(Guid userId)
    {
        var sql = "UPDATE users SET failed_login_attempts = 0, lockout_end = NULL WHERE id = @UserId";
        await ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var sql = "UPDATE users SET last_login_at = @LastLoginAt WHERE id = @UserId";
        await ExecuteAsync(sql, new { UserId = userId, LastLoginAt = DateTime.UtcNow });
    }

    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        var sql = "SELECT * FROM users WHERE password_reset_token = @Token";
        return await QueryFirstOrDefaultAsync(sql, MapUser, new { Token = token });
    }

    private User MapUser(IDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("password_hash")),
            FirstName = reader.GetString(reader.GetOrdinal("first_name")),
            LastName = reader.GetString(reader.GetOrdinal("last_name")),
            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? null : reader.GetString(reader.GetOrdinal("phone_number")),
            Role = reader.GetString(reader.GetOrdinal("role")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            IsEmailVerified = reader.GetBoolean(reader.GetOrdinal("is_email_verified")),
            EmailVerificationToken = reader.IsDBNull(reader.GetOrdinal("email_verification_token")) ? null : reader.GetString(reader.GetOrdinal("email_verification_token")),
            PasswordResetToken = reader.IsDBNull(reader.GetOrdinal("password_reset_token")) ? null : reader.GetString(reader.GetOrdinal("password_reset_token")),
            PasswordResetExpires = reader.IsDBNull(reader.GetOrdinal("password_reset_expires")) ? null : reader.GetDateTime(reader.GetOrdinal("password_reset_expires")),
            FailedLoginAttempts = reader.GetInt32(reader.GetOrdinal("failed_login_attempts")),
            LockoutEnd = reader.IsDBNull(reader.GetOrdinal("lockout_end")) ? null : reader.GetDateTime(reader.GetOrdinal("lockout_end")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
            LastLoginAt = reader.IsDBNull(reader.GetOrdinal("last_login_at")) ? null : reader.GetDateTime(reader.GetOrdinal("last_login_at"))
        };
    }
}
