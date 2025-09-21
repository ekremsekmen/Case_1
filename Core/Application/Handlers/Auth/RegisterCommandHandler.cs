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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<RegisterCommandHandler> _logger;

        public RegisterCommandHandler(
            ApplicationDbContext context, 
            IJwtService jwtService,
            ILogger<RegisterCommandHandler> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this username or email already exists");
            }

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("New user registered: {Username}", user.Username);

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token
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
