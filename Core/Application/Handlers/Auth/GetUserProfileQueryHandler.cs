using Case_1_2.Core.Application.DTOs.Auth;
using Case_1_2.Core.Application.Queries.Auth;
using Case_1_2.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Case_1_2.Core.Application.Handlers.Auth
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserDto?>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetUserProfileQueryHandler> _logger;

        public GetUserProfileQueryHandler(
            ApplicationDbContext context,
            ILogger<GetUserProfileQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserDto?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Handling GetUserProfileQuery for UserId: {UserId}", request.UserId);

                var user = await _context.Users
                    .Where(u => u.Id == request.UserId)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        CreatedAt = u.CreatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User with Id {UserId} not found", request.UserId);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved user profile for UserId: {UserId}", request.UserId);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetUserProfileQuery for UserId: {UserId}", request.UserId);
                throw;
            }
        }
    }
}
