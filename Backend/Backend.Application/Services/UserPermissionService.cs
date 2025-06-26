using AutoMapper;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Backend.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Services
{
    public class UserPermissionService : IUserPermissions
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<UserPermission> _genericRepository;
        private readonly ILogger<UserPermissionService> _logger;
        private readonly DataBaseContext _context;

        public UserPermissionService(IMapper mapper,
            IGenericRepository<UserPermission> genericRepository,
            ILogger<UserPermissionService> logger,
            DataBaseContext context
            )
        {
            _mapper = mapper;
            _genericRepository = genericRepository;
            _logger = logger;
            _context = context;
        }
        public async Task<object> AddUserPermissionAsync(UserPermissionDTO dto)
        {
            _logger.LogInformation("Adding multiple UserPermissions for user {UserId}.", dto.UserId);
            try
            {
                var permissions = dto.PageUrl.Select(url => new UserPermission
                {
                    UserId = dto.UserId,
                    PageUrl = url,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
                await _genericRepository.AddRangeAsync(permissions);

                return new { Success = true, Message = "Permissions added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding permissions for user {UserId}", dto.UserId);
                throw;
            }
        }

        public async Task DeleteUserPermissionAsync(int userPermissionId)
        {
            _logger.LogInformation("Soft deleting UserPermission with ID {userPermissionId}.", userPermissionId);
            try
            {
                var userPermission = await _genericRepository.GetByIdAsync(userPermissionId);
                if (userPermission == null)
                {
                    _logger.LogWarning("UserPermission with ID {userPermissionId} not found.", userPermissionId);
                    return;
                }

                userPermission.IsActive = false;
                await _genericRepository.UpdateAsync(userPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error soft deleting userPermission with ID {userPermissionId}: {Exception}", ex, userPermissionId);
                throw;
            }
        }

        public async Task<List<GetUserPermissionDTO>> GetAllUserPermissionsAsync()
        {
            _logger.LogInformation("Fetching all userPermission.");
            try
            {
                var userPermissions = await _genericRepository.GetAllAsync();
                return _mapper.Map<List<GetUserPermissionDTO>>(userPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching userPermission: {Exception}", ex);
                throw;
            }
        }

        public async Task<GetUserPermissionDTO> GetUserPermissionByIdAsync(int userPermissionId)
        {
            _logger.LogInformation("Fetching UserPermission with ID {userPermissionId}.", userPermissionId);
            try
            {
                var userPermission = await _genericRepository.GetByIdAsync(userPermissionId);
                return _mapper.Map<GetUserPermissionDTO>(userPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching userPermissions with ID {UserPermissionId}: {Exception}", ex, userPermissionId);
                throw;
            }
        }

        public async Task<List<GetUserPermissionDTO>> GetUserPermissionByUserIdAsync(int userId)
        {
            _logger.LogInformation("Fetching UserPermission with ID {userId}.", userId);
            try
            {
                var userPermissions = await _genericRepository.GetAllAsync(
                    (x => x.UserId == userId),
                    include: q => q
                    .Include(a => a.Users)
                    );
                return _mapper.Map<List<GetUserPermissionDTO>>(userPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching userPermissions with ID {userId}: {Exception}", ex, userId);
                throw;
            }
        }

        public async Task UpdateUserPermissionAsync(UserPermissionDTO dto)
        {
            _logger.LogInformation("Updating permissions for user {UserId}.", dto.UserId);

            // Begin transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Delete existing permissions for the user
                var existingPermissions = await _genericRepository.FindAllAsync(p => p.UserId == dto.UserId);

                if (existingPermissions.Any())
                {
                    await _genericRepository.DeleteRangeAsync(existingPermissions);
                }

                // 2. Add new permissions
                var newPermissions = dto.PageUrl.Select(url => new UserPermission
                {
                    UserId = dto.UserId,
                    PageUrl = url,
                    IsActive = dto.IsActive,
                    CreatedBy = dto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _genericRepository.AddRangeAsync(newPermissions);

                // Commit transaction
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully updated permissions for user {UserId}.", dto.UserId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating permissions for user {UserId}", dto.UserId);
                throw;
            }
        }

    }
}
