using Backend.Application.DTOs;

namespace Backend.Application.Interfaces
{
    public interface IUserRoles
    {
        Task<List<UserRoleDTO>> GetAllUserRolesAsync();
        Task<UserRoleDTO> GetUserRoleByIdAsync(int userRoleId);
        Task<object> AddUserRoleAsync(UserRoleDTO userRole);
        Task UpdateUserRoleAsync(UserRoleDTO userRole);
        Task DeleteUserRoleAsync(int userRoleId);
    }
}
