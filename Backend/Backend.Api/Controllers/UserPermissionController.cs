using Backend.Api.Models;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionController : ControllerBase
    {
        private readonly ILogger<UserPermissionController> _logger;
        private readonly IUserPermissions _userPermissions;

        public UserPermissionController(ILogger<UserPermissionController> logger, IUserPermissions userPermissions)
        {
            _logger = logger;
            _userPermissions = userPermissions;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserPermission([FromBody] UserPermissionDTO dto)
        {
            _logger.LogInformation("Adding a new UserPermission with name {PageUrl}.", dto.PageUrl);
            try
            {
                await _userPermissions.AddUserPermissionAsync(dto);
                _logger.LogInformation("Successfully added userPermission with ID {UserPermissionId}.", dto.UserPermissionId);
                return Ok(ApiResponse<UserPermissionDTO>.SuccessResponse(dto, "UserPermission {UserPermission} added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new userPermission.");
                return StatusCode(500, ApiResponse<UserPermissionDTO>.ErrorResponse("Internal server error."));
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<GetUserPermissionDTO>>> GetAllUserPermissions()
        {
            _logger.LogInformation("Fetcing All UserPermissions");
            try
            {
                var userPermissions = await _userPermissions.GetAllUserPermissionsAsync();
                _logger.LogInformation("Successfully retrieved {Count} userPermissions .", userPermissions?.Count() ?? 0);
                return Ok(ApiResponse<IEnumerable<GetUserPermissionDTO>>.SuccessResponse(userPermissions, "userPermissions retrieved successfully"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while fetching all userPermissions .");
                return StatusCode(500, ApiResponse<IEnumerable<GetUserPermissionDTO>>.ErrorResponse("Internal server error."));
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<GetUserPermissionDTO>>> GetUserPermissionsByUserId(int userId)
        {
            _logger.LogInformation("Fetcing All UserPermissions");
            try
            {
                var userPermissions = await _userPermissions.GetUserPermissionByUserIdAsync(userId);
                _logger.LogInformation("Successfully retrieved userPermissions .");
                return Ok(ApiResponse<IEnumerable<GetUserPermissionDTO>>.SuccessResponse(userPermissions, "userPermissions retrieved successfully"));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while fetching all userPermissions .");
                return StatusCode(500, ApiResponse<IEnumerable<GetUserPermissionDTO>>.ErrorResponse("Internal server error."));
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateUserPermission(UserPermissionDTO dto)
        {
            _logger.LogInformation("Update UserPermission {dto.UserPermissionId}.", dto.UserPermissionId);
            try
            {
                await _userPermissions.UpdateUserPermissionAsync(dto);
                _logger.LogInformation("Update userPermission {dto.PageUrl}.", dto.UserPermissionId);
                return Ok(ApiResponse<UserPermissionDTO>.SuccessResponse(dto, "UserPermissions Updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating userPermission  with ID {dto.PageUrl}.", dto.UserPermissionId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error."));
            }
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUserPermission(int userPermissionId)
        {

            _logger.LogInformation("Deleting userPermission with ID {userPermissionId}.", userPermissionId);
            try
            {
                await _userPermissions.DeleteUserPermissionAsync(userPermissionId);
                _logger.LogInformation("Successfully deleted userPermission with ID {userPermissionId}.", userPermissionId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "userPermission deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting User Role with ID {userPermissionId}.", userPermissionId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error."));
            }

        }
    }
}
