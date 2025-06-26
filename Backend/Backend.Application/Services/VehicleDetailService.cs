using AutoMapper;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services
{
    public class VehicleDetailService : IVehicleDetails
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<VehicleDetail> _genericRepository;
        private readonly ILogger<VehicleDetailService> _logger;

        public VehicleDetailService(IMapper mapper, IGenericRepository<VehicleDetail> genericRepository, ILogger<VehicleDetailService> logger)
        {
            _mapper = mapper;
            _genericRepository = genericRepository;
            _logger = logger;
        }
        public async Task<object> AddVehicleDetailAsync(VehicleDetailDTO vehicleDetail)
        {
            _logger.LogInformation("Adding new VehicleDetail {Name}.", vehicleDetail.Name);
            try
            {
                var newUser = _mapper.Map<VehicleDetail>(vehicleDetail);
                await _genericRepository.AddAsync(newUser);

                return new { Success = true, Message = "VehicleDetail added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding vehicleDetail {Name}.", vehicleDetail.Name);
                throw;
            }
        }

        public async Task DeleteVehicleDetailAsync(int vehicleDetailId)
        {
            _logger.LogInformation("Soft deleting VehicleDetail with ID {vehicleDetailId}.", vehicleDetailId);
            try
            {
                var vehicleDetail = await _genericRepository.GetByIdAsync(vehicleDetailId);
                if (vehicleDetail == null)
                {
                    _logger.LogWarning("VehicleDetail with ID {vehicleDetailId} not found.", vehicleDetailId);
                    return;
                }

                vehicleDetail.IsActive = false;
                await _genericRepository.UpdateAsync(vehicleDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error soft deleting vehicleDetail with ID {vehicleDetailId}: {Exception}", ex, vehicleDetailId);
                throw;
            }
        }

        public async Task<List<VehicleDetailDTO>> GetAllVehicleDetailsAsync()
        {
            _logger.LogInformation("Fetching all VehicleDetails.");
            try
            {
                var vehicleDetails = await _genericRepository.GetAllAsync();
                return _mapper.Map<List<VehicleDetailDTO>>(vehicleDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching VehicleDetails: {Exception}", ex);
                throw;
            }
        }

        public async Task<VehicleDetailDTO> GetVehicleDetailByIdAsync(int vehicleDetailId)
        {
            _logger.LogInformation("Fetching VehicleDetail with ID {vehicleDetailId}.", vehicleDetailId);
            try
            {
                var user = await _genericRepository.GetByIdAsync(vehicleDetailId);
                return _mapper.Map<VehicleDetailDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching user role with ID {VehicleDetailId}: {Exception}", ex, vehicleDetailId);
                throw;
            }
        }

        public async Task UpdateVehicleDetailAsync(VehicleDetailDTO dto)
        {
            _logger.LogInformation("Updating vehicleDetail with ID {VehicleDetailId}.", dto.VehicleDetailId);
            try
            {
                var vehicleDetail = _mapper.Map<VehicleDetail>(dto);
                await _genericRepository.UpdateAsync(vehicleDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating vehicleDetail with ID {VehicleDetailId}: {Exception}", ex, dto.VehicleDetailId);
                throw;
            }
        }
    }
}
