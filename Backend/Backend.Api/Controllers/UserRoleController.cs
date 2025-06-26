using Backend.Api.Models;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly ILogger<UserRoleController> _logger;
        private readonly IUserRoles _userRoles;

        public UserRoleController(ILogger<UserRoleController> logger, IUserRoles userRoles)
        {
            _logger = logger;
            _userRoles = userRoles;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserRole([FromBody] UserRoleDTO dto)
        {
            _logger.LogInformation("Adding a new UserRole with name {RoleName}.", dto.RoleName);
            try
            {
                await _userRoles.AddUserRoleAsync(dto);
                _logger.LogInformation("Successfully added userRole with ID {UserRoleId}.", dto.UserRoleId);
                return Ok(ApiResponse<UserRoleDTO>.SuccessResponse(dto, "UserRole {UserRole} added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new userRole.");
                return StatusCode(500, ApiResponse<UserRoleDTO>.ErrorResponse("Internal server error."));
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<UserRoleDTO>>> GetAllUserRoles()
        {
            _logger.LogInformation("Fetcing All User Roles");
            try
            {
                var userRoles = await _userRoles.GetAllUserRolesAsync();
                _logger.LogInformation("Successfully retrieved {Count} userRoles .", userRoles?.Count() ?? 0);
                return Ok(ApiResponse<IEnumerable<UserRoleDTO>>.SuccessResponse(userRoles, "userRoles retrieved successfully"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while fetching all userRoles .");
                return StatusCode(500, ApiResponse<IEnumerable<UserRoleDTO>>.ErrorResponse("Internal server error."));
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserRole(UserRoleDTO dto)
        {
            _logger.LogInformation("Update UserRole {roleName}.", dto.UserRoleId);
            try
            {
                await _userRoles.UpdateUserRoleAsync(dto);
                _logger.LogInformation("Update userRole {RoleName}.", dto.UserRoleId);
                return Ok(ApiResponse<UserRoleDTO>.SuccessResponse(dto, "User Role Updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating userRole  with ID {RoleName}.", dto.UserRoleId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error."));
            }
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUserRole(int userRoleId)
        {

            _logger.LogInformation("Deleting userRole with ID {userRoleId}.", userRoleId);
            try
            {
                await _userRoles.DeleteUserRoleAsync(userRoleId);
                _logger.LogInformation("Successfully deleted userRole with ID {userRoleId}.", userRoleId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "userRole deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting User Role with ID {userRoleId}.", userRoleId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error."));
            }

        }
    }
}
