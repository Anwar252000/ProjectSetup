using Backend.Application.DTOs;

namespace Backend.Application.Interfaces
{
    public interface IUserPermissions
    {
        Task<List<GetUserPermissionDTO>> GetAllUserPermissionsAsync();
        Task<GetUserPermissionDTO> GetUserPermissionByIdAsync(int userPermissionId);
        Task<List<GetUserPermissionDTO>> GetUserPermissionByUserIdAsync(int userId);
        Task<object> AddUserPermissionAsync(UserPermissionDTO userPermission);
        Task UpdateUserPermissionAsync(UserPermissionDTO userPermission);
        Task DeleteUserPermissionAsync(int userPermissionId);
    }
}
