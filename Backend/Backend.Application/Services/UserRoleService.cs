using AutoMapper;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services
{
    public class UserRoleService : IUserRoles
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<UserRole> _genericRepository;
        private readonly ILogger<UserRoleService> _logger;

        public UserRoleService(IMapper mapper, IGenericRepository<UserRole> genericRepository, ILogger<UserRoleService> logger)
        {
            _mapper = mapper;
            _genericRepository = genericRepository;
            _logger = logger;
        }
        public async Task<object> AddUserRoleAsync(UserRoleDTO userRole)
        {
            _logger.LogInformation("Adding new UserRole {RoleName}.", userRole.RoleName);
            try
            {
                var newUser = _mapper.Map<UserRole>(userRole);
                await _genericRepository.AddAsync(newUser);

                return new { Success = true, Message = "UserRole added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding userRole {RoleName}.", userRole.RoleName);
                throw;
            }
        }

        public async Task DeleteUserRoleAsync(int userRoleId)
        {
            _logger.LogInformation("Soft deleting UserRole with ID {userRoleId}.", userRoleId);
            try
            {
                var userRole = await _genericRepository.GetByIdAsync(userRoleId);
                if (userRole == null)
                {
                    _logger.LogWarning("UserRole with ID {userRoleId} not found.", userRoleId);
                    return;
                }

                userRole.IsActive = false;
                await _genericRepository.UpdateAsync(userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error soft deleting userRole with ID {userRoleId}: {Exception}", ex, userRoleId);
                throw;
            }
        }

        public async Task<List<UserRoleDTO>> GetAllUserRolesAsync()
        {
            _logger.LogInformation("Fetching all user roles.");
            try
            {
                var userRoles = await _genericRepository.GetAllAsync();
                return _mapper.Map<List<UserRoleDTO>>(userRoles);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching user roles: {Exception}", ex);
                throw;
            }
        }

        public async Task<UserRoleDTO> GetUserRoleByIdAsync(int userRoleId)
        {
            _logger.LogInformation("Fetching UserRole with ID {userRoleId}.", userRoleId);
            try
            {
                var user = await _genericRepository.GetByIdAsync(userRoleId);
                return _mapper.Map<UserRoleDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching user role with ID {UserRoleId}: {Exception}", ex, userRoleId);
                throw;
            }
        }

        public async Task UpdateUserRoleAsync(UserRoleDTO dto)
        {
            _logger.LogInformation("Updating userRole with ID {UserRoleId}.", dto.UserRoleId);
            try
            {
                var userRole = _mapper.Map<UserRole>(dto);
                await _genericRepository.UpdateAsync(userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating userRole with ID {UserRoleId}: {Exception}", ex, dto.UserRoleId);
                throw;
            }
        }
    }
}
