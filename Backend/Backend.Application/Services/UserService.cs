using AutoMapper;
using Backend.Application.DTOs;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Backend.Application.Services
{
    public class UserService : IUsers
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IJwtToken _jwtToken;

        public UserService(
            IMapper mapper, 
            IGenericRepository<User> userRepository,
            IGenericRepository<RefreshToken> refreshTokenRepository,
            ILogger<UserService> logger, IJwtToken jwtToken
            )
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
            _jwtToken = jwtToken;
        }
        public async Task<object> AddUserAsync(UserDTO dto)
        {
            _logger.LogInformation("Adding new User {UserName}.", dto.FullName);
            try
            {
                var existingUser = await GetUserByUserNameAsync(dto.UserName);

                if (existingUser != null)
                {
                    throw new Exception("User name already exists.");
                }

                var newUser = _mapper.Map<User>(dto);

                // Hash the password before saving
                var hasher = new PasswordHasher<User>();
                newUser.Password = hasher.HashPassword(newUser, dto.Password);

                await _userRepository.AddAsync(newUser);

                return new { Success = true, Message = "User added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user {UserName}.", dto.UserName);
                throw;
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            _logger.LogInformation("Soft deleting User with ID {UserId}.", userId);
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return;
                }

                user.IsActive = false;
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error soft deleting user with ID {UserId}: {Exception}", ex, userId);
                throw;
            }
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users.");
            try
            {
                var users = await _userRepository.GetAllAsync(
                    include: q => q.Include(a => a.UserRoles)
                    );
                return _mapper.Map<List<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching users: {Exception}", ex);
                throw;
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(int userId)
        {
            _logger.LogInformation("Fetching User with ID {UserId}.", userId);
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching user with ID {UserId}: {Exception}", ex, userId);
                throw;
            }
        }

        public async Task<UserDTO> GetUserByUserNameAsync(string userName)
        {
            _logger.LogInformation("Fetching User with UserName {userName}.", userName);

            try
            {
                var user = await _userRepository.GetByConditionAsync(
                    (x => x.UserName == userName),
                    include: q => q
                    .Include(a => a.UserRoles)
                    .Include(a => a.UserPermissions)
                    );
                var dto = _mapper.Map<UserDTO>(user);

                if (dto != null)
                {
                    _logger.LogInformation("User with userName {userName} retrieved.", userName);
                }
                else
                {
                    _logger.LogWarning("User with ID {userName} not found.", userName);
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while fetching user with Username: {Username}", ex, userName);
                throw;
            }
        }

        public async Task<UserDTO> Login(LoginDTO dto)
        {
            _logger.LogInformation("Attempting login for username: {Username}", dto.UserName);
            try
            {
                var userDto = await GetUserByUserNameAsync(dto.UserName);
                var user = _mapper.Map<User>(userDto);

                if (user == null || !VerifyPassword(user, dto.Password))
                {
                    _logger.LogWarning("Invalid credentials for UserName: {UserName}", dto.UserName);
                    throw new Exception("Invalid credentials.");
                }

                // Create claims
                var claims = new List<Claim>
                {
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                var accessToken = _jwtToken.GenerateJwtToken(claims);
                var refreshToken = _jwtToken.GenerateRefreshToken();

                // Store the refresh token in DB
                var refreshTokenEntry = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.UserId,
                    Expires = DateTime.UtcNow.AddHours(5),
                    IsRevoked = false
                };

                await _refreshTokenRepository.AddAsync(refreshTokenEntry);
                userDto.Token = accessToken;
                userDto.RefreshToken = refreshToken; // Add this to your DTO
                return userDto;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateUserAsync(UserDTO dto)
        {
            _logger.LogInformation("Updating User with ID {UserId}.", dto.UserId);
            try
            {
                var user = _mapper.Map<User>(dto);
                // Hash the password before saving
                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, dto.Password);
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating user with ID {UserId}: {Exception}", ex, dto.UserId);
                throw;
            }
        }

        private bool VerifyPassword(User user, string password)
        {
            var hasher = new PasswordHasher<User>();

            // Assuming user.Password is the stored hash of the user's password
            var verificationResult = hasher.VerifyHashedPassword(user, user.Password, password);

            // Check if the password is correct
            return verificationResult == PasswordVerificationResult.Success;
        }

    }
}
