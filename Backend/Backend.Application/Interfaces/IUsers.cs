using Backend.Application.DTOs;

namespace Backend.Application.Interfaces
{
    public interface IUsers
    {
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int userId);
        Task<object> AddUserAsync(UserDTO user);
        Task<UserDTO> Login(LoginDTO user);
        Task<UserDTO> GetUserByUserNameAsync(string userName);
        Task UpdateUserAsync(UserDTO user);
        Task DeleteUserAsync(int userId);
    }
}
