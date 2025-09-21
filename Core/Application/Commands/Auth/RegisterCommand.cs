using Case_1_2.Core.Application.DTOs.Auth;
using MediatR;

namespace Case_1_2.Core.Application.Commands.Auth
{
    public class RegisterCommand : IRequest<AuthResponseDto>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
