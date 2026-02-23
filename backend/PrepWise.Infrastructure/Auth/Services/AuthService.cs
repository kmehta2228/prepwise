using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrepWise.Application.Auth.DTOs;
using PrepWise.Application.Auth.Exceptions;
using PrepWise.Application.Auth.Interfaces;
using PrepWise.Domain.Entities;
using PrepWise.Infrastructure.Data;

namespace PrepWise.Infrastructure.Auth.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        AppDbContext dbContext,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var emailExists = await _dbContext.Users.AnyAsync(
            user => user.Email.ToLower() == normalizedEmail,
            cancellationToken);

        if (emailExists)
        {
            throw new DuplicateEmailException(request.Email);
        }

        var user = new User
        {
            Email = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            Token = _jwtTokenService.GenerateToken(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            user => user.Email.ToLower() == normalizedEmail,
            cancellationToken);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verifyResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException();
        }

        return new AuthResponse
        {
            Token = _jwtTokenService.GenerateToken(user)
        };
    }
}
