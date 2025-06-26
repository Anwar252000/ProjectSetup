using Backend.Api.Models;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers _users;
        private readonly IJwtToken _jwtToken;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;

        public AuthController(ILogger<AuthController> logger,
            IConfiguration configuration,
            IUsers users,
            IJwtToken jwtToken,
            IGenericRepository<RefreshToken> refreshTokenRepository
            )
        {
            _logger = logger;
            _configuration = configuration;
            _users = users;
            _jwtToken = jwtToken;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] UserDTO dto)
        {
            _logger.LogInformation("Adding a new User with name {userName}.", dto.UserName);
            try
            {
                await _users.AddUserAsync(dto);
                _logger.LogInformation("Successfully added user with ID {userId}.", dto.UserId);
                return Ok(ApiResponse<UserDTO>.SuccessResponse(dto, "User {userName} added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new user.");
                // If the exception is a known case like "User already exists"
                if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(ApiResponse<UserDTO>.ErrorResponse(ex.Message));
                }
                return StatusCode(500, ApiResponse<UserDTO>.ErrorResponse("Internal server error."));
            }
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var user = await _users.Login(dto);
                return Ok(ApiResponse<object>.SuccessResponse(user, "Login successful."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Login failed.",
                    new List<string> { ex.Message }));
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            _logger.LogInformation("Fetcing All Users");
            try
            {
                var users = await _users.GetAllUsersAsync();
                _logger.LogInformation("Successfully retrieved {Count} users .", users?.Count() ?? 0);
                return Ok(ApiResponse<IEnumerable<UserDTO>>.SuccessResponse(users, "users retrieved successfully"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while fetching all user .");
                return StatusCode(500, ApiResponse<IEnumerable<UserDTO>>.ErrorResponse("Internal server error."));
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUser(UserDTO dto)
        {
            _logger.LogInformation("Update user {userName}.", dto.UserId);
            try
            {
                await _users.UpdateUserAsync(dto);
                _logger.LogInformation("Update user {userName}.", dto.UserId);
                return Ok(ApiResponse<UserDTO>.SuccessResponse(dto, "User  Updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user  with ID {userName}.", dto.UserId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error."));
            }
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUser(int userId)
        {

            _logger.LogInformation("Deleting user with ID {userId}.", userId);
            try
            {
                await _users.DeleteUserAsync(userId);
                _logger.LogInformation("Successfully deleted User with ID {userId}.", userId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "User deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting User Role with ID {userId}.", userId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error."));
            }

        }
        private string GenerateJwtToken(UserDTO user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.UserRoleName),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Refresh token is required.");

            var storedToken = await _refreshTokenRepository.FindAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked);

            if (storedToken == null || storedToken.Expires < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            // Revoke old token
            storedToken.IsRevoked = true;

            // Retrieve the user
            var user = await _users.GetUserByIdAsync(storedToken.UserId);
            if (user == null)
                return Unauthorized("User not found.");

            // Generate new tokens
            var claims = new List<Claim>
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var newAccessToken = _jwtToken.GenerateJwtToken(claims);
            var newRefreshToken = _jwtToken.GenerateRefreshToken();

            // Save new refresh token
            var refreshTokenEntry = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.UserId,
                Expires = DateTime.UtcNow.AddHours(5),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntry);

            return Ok(new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });

        }
    }
}