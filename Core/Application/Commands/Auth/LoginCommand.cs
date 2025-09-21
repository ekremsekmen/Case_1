using Case_1_2.Core.Application.DTOs.Auth;
using MediatR;

namespace Case_1_2.Core.Application.Commands.Auth
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
