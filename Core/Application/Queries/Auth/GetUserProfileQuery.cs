using Case_1_2.Core.Application.DTOs.Auth;
using MediatR;

namespace Case_1_2.Core.Application.Queries.Auth
{
    public class GetUserProfileQuery : IRequest<UserDto?>
    {
        public int UserId { get; set; }

        public GetUserProfileQuery(int userId)
        {
            UserId = userId;
        }
    }
}
