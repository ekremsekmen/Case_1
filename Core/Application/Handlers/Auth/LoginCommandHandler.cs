using Case_1_2.Core.Application.Commands.Auth;
using Case_1_2.Core.Application.DTOs.Auth;
using Case_1_2.Core.Domain.Entities;
using Case_1_2.Infrastructure.Data;
using Case_1_2.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Case_1_2.Core.Application.Handlers.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            ApplicationDbContext context,
            IJwtService jwtService,
            ILogger<LoginCommandHandler> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Find user by username or email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => 
                    (u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail) 
                    && u.IsActive, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Login attempt with invalid username/email: {UsernameOrEmail}", request.UsernameOrEmail);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login attempt with invalid password for user: {Username}", user.Username);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            _logger.LogInformation("User logged in successfully: {Username}", user.Username);

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token (remove old ones first)
            var existingTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == user.Id)
                .ToListAsync(cancellationToken);

            _context.RefreshTokens.RemoveRange(existingTokens);

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            };
        }
    }
}
