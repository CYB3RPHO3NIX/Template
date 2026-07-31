using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.CommandHandler;
using Template.Database.Domain.Contexts;
using Template.Services.Jwt;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Exceptions;
using Template.Utilities.Cryptography;

namespace Template.CommandHandlers.Identity.UserCommandHandlers;

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse?>
{
    private readonly TemplateDbContext _dbContext;
    private readonly IJwtTokenService _tokenService;

    public LoginCommandHandler(TemplateDbContext dbContext, IJwtTokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse?> Handle(LoginCommand command)
    {
        using (LogContext.PushProperty("TraceId", command.TraceId))
        {
            Log.Information("Handling LoginCommand for email: {Email}", command.Email);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == command.Email);

            if (user == null)
            {
                Log.Warning("Login failed: User not found with email {Email}", command.Email);
                throw new BusinessLogicException("Invalid email or password", "INVALID_CREDENTIALS");
            }

            if (!user.IsActive)
            {
                Log.Warning("Login failed: User is inactive, email: {Email}", command.Email);
                throw new BusinessLogicException("User account is inactive", "USER_INACTIVE");
            }

            var passwordHash = HashGenerator.GenerateSHA256Hash(command.Password, user.PasswordSalt);
            if (passwordHash != user.PasswordHash)
            {
                Log.Warning("Login failed: Invalid password for email {Email}", command.Email);
                throw new BusinessLogicException("Invalid email or password", "INVALID_CREDENTIALS");
            }

            var (accessToken, refreshToken, expiresIn) = _tokenService.GenerateTokens(
                user.UserId,
                user.Email,
                user.Username);

            Log.Information("Login successful for user {UserId}", user.UserId);

            return new LoginResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                UserName = user.Username,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn
            };
        }
    }
}
