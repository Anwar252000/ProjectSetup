using Backend.Api.Models;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleDetailController : ControllerBase
    {
        private readonly ILogger<VehicleDetailController> _logger;
        private readonly IVehicleDetails _vehicleDetails;

        public VehicleDetailController(ILogger<VehicleDetailController> logger, IVehicleDetails vehicleDetails)
        {
            _logger = logger;
            _vehicleDetails = vehicleDetails;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddVehicleDetail([FromBody] VehicleDetailDTO dto)
        {
            _logger.LogInformation("Adding a new VehicleDetail with name {Name}.", dto.Name);
            try
            {
                await _vehicleDetails.AddVehicleDetailAsync(dto);
                _logger.LogInformation("Successfully added VehicleDetail with ID {VehicleDetailId}.", dto.VehicleDetailId);
                return Ok(ApiResponse<VehicleDetailDTO>.SuccessResponse(dto, "VehicleDetail {VehicleDetail} added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new VehicleDetail.");
                return StatusCode(500, ApiResponse<VehicleDetailDTO>.ErrorResponse("Internal server error."));
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<VehicleDetailDTO>>> GetAllVehicleDetails()
        {
            _logger.LogInformation("Fetcing All vehicleDetails");
            try
            {
                var vehicleDetails = await _vehicleDetails.GetAllVehicleDetailsAsync();
                _logger.LogInformation("Successfully retrieved {Count} vehicleDetails .", vehicleDetails?.Count() ?? 0);
                return Ok(ApiResponse<IEnumerable<VehicleDetailDTO>>.SuccessResponse(vehicleDetails, "VehicleDetails retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all vehicleDetails .");
                return StatusCode(500, ApiResponse<IEnumerable<VehicleDetailDTO>>.ErrorResponse("Internal server error."));
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateVehicleDetail(VehicleDetailDTO dto)
        {
            _logger.LogInformation("Update VehicleDetail {Name}.", dto.Name);
            try
            {
                await _vehicleDetails.UpdateVehicleDetailAsync(dto);
                _logger.LogInformation("Update VehicleDetail {Name}.", dto.Name);
                return Ok(ApiResponse<VehicleDetailDTO>.SuccessResponse(dto, "VehicleDetail Updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating VehicleDetail  with ID {Name}.", dto.Name);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error."));
            }
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteVehicleDetail(int VehicleDetailId)
        {

            _logger.LogInformation("Deleting VehicleDetail with ID {VehicleDetailId}.", VehicleDetailId);
            try
            {
                await _vehicleDetails.DeleteVehicleDetailAsync(VehicleDetailId);
                _logger.LogInformation("Successfully deleted VehicleDetail with ID {VehicleDetailId}.", VehicleDetailId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "VehicleDetail deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting vehicleDetails with ID {VehicleDetailId}.", VehicleDetailId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error."));
            }

        }
    }
}
