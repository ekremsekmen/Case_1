using System.ComponentModel.DataAnnotations;

namespace Case_1_2.Core.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
