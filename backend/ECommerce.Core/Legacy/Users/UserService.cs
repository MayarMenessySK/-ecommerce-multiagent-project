using AutoMapper;
using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Core.Users;

public class UserService : BaseService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly string _jwtSecret;

    public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _configuration = configuration;
        _jwtSecret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
    }

    public async Task<AuthResponse> RegisterAsync(RegisterInput input)
    {
        if (await _userRepository.EmailExistsAsync(input.Email))
        {
            throw new InvalidOperationException("Email already exists");
        }

        if (!input.AcceptTerms)
        {
            throw new InvalidOperationException("You must accept the terms and conditions");
        }

        var user = _mapper.Map<User>(input);
        user.PasswordHash = CryptographyHelper.HashPassword(input.Password);
        user.EmailVerificationToken = CryptographyHelper.GenerateRandomToken();
        user.IsEmailVerified = false;
        user.Role = "Customer";

        user = await _userRepository.CreateAsync(user);

        var accessToken = CryptographyHelper.GenerateJwtToken(user.Id.ToString(), user.Email, user.Role, _jwtSecret, 15);
        var refreshToken = CryptographyHelper.GenerateRefreshToken();

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Token = new TokenData
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 900,
                TokenType = "Bearer"
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginInput input)
    {
        var user = await _userRepository.GetByEmailAsync(input.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Account is locked. Please try again later.");
        }

        if (!CryptographyHelper.VerifyPassword(input.Password, user.PasswordHash))
        {
            await _userRepository.IncrementFailedLoginAttemptsAsync(user.Id);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("Account is inactive");
        }

        await _userRepository.ResetFailedLoginAttemptsAsync(user.Id);
        await _userRepository.UpdateLastLoginAsync(user.Id);

        var expiresIn = input.RememberMe ? 43200 : 15;
        var accessToken = CryptographyHelper.GenerateJwtToken(user.Id.ToString(), user.Email, user.Role, _jwtSecret, expiresIn);
        var refreshToken = CryptographyHelper.GenerateRefreshToken();

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Token = new TokenData
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn * 60,
                TokenType = "Bearer"
            }
        };
    }

    public async Task<UserOutput> GetProfileAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            ThrowNotFound("User", userId);
        }

        return _mapper.Map<UserOutput>(user!);
    }

    public async Task<UserOutput> UpdateProfileAsync(Guid userId, UpdateProfileInput input)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            ThrowNotFound("User", userId);
        }

        user!.FirstName = input.FirstName;
        user.LastName = input.LastName;
        user.PhoneNumber = input.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        user = await _userRepository.UpdateAsync(user);
        return _mapper.Map<UserOutput>(user);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordInput input)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            ThrowNotFound("User", userId);
        }

        if (!CryptographyHelper.VerifyPassword(input.CurrentPassword, user!.PasswordHash))
        {
            throw new InvalidOperationException("Current password is incorrect");
        }

        user.PasswordHash = CryptographyHelper.HashPassword(input.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
    }

    public async Task RequestPasswordResetAsync(ForgotPasswordInput input)
    {
        var user = await _userRepository.GetByEmailAsync(input.Email);
        if (user == null)
        {
            return;
        }

        user.PasswordResetToken = CryptographyHelper.GenerateRandomToken();
        user.PasswordResetExpires = DateTime.UtcNow.AddHours(1);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
    }

    public async Task ResetPasswordAsync(ResetPasswordInput input)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(input.Token);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid or expired reset token");
        }

        if (user.PasswordResetExpires < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Reset token has expired");
        }

        user.PasswordHash = CryptographyHelper.HashPassword(input.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetExpires = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
    }
}
