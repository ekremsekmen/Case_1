using Case_1_2.Core.Application.DTOs.Auth;
using Case_1_2.Core.Application.Commands.Auth;
using Case_1_2.Core.Application.Queries.Auth;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Case_1_2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registerDto">User registration data</param>
        /// <returns>Authentication response with tokens</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var command = new RegisterCommand
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    Password = registerDto.Password,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(Register), result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration failed: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration");
                return StatusCode(500, "An error occurred during registration");
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>Authentication response with tokens</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var command = new LoginCommand
                {
                    UsernameOrEmail = loginDto.UsernameOrEmail,
                    Password = loginDto.Password
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Login failed: {Message}", ex.Message);
                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, "An error occurred during login");
            }
        }

        /// <summary>
        /// Get current user profile (requires authentication)
        /// </summary>
        /// <returns>User profile information</returns>
        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                {
                    _logger.LogWarning("Invalid or missing userId in token");
                    return Unauthorized();
                }

                _logger.LogInformation("Getting user profile for UserId: {UserId} via CQRS", userIdInt);
                var query = new GetUserProfileQuery(userIdInt);
                var userProfile = await _mediator.Send(query);

                if (userProfile == null)
                {
                    _logger.LogWarning("User profile not found for UserId: {UserId}", userIdInt);
                    return NotFound("User profile not found");
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user profile");
                return StatusCode(500, "An error occurred while getting profile");
            }
        }
    }
}
