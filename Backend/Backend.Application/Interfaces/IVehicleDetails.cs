using Backend.Application.DTOs;

namespace Backend.Application.Interfaces
{
    public interface IVehicleDetails
    {
        Task<List<VehicleDetailDTO>> GetAllVehicleDetailsAsync();
        Task<VehicleDetailDTO> GetVehicleDetailByIdAsync(int vehicleDetailId);
        Task<object> AddVehicleDetailAsync(VehicleDetailDTO vehicleDetail);
        Task UpdateVehicleDetailAsync(VehicleDetailDTO vehicleDetail);
        Task DeleteVehicleDetailAsync(int vehicleDetailId);
    }
}
