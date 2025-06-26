using Backend.Domain.Entities;

namespace Backend.Application.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public int? UserRoleId { get; set; }
        public string? UserRoleName { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual UserRole? UserRoles { get; set; }
        public virtual ICollection<UserPermission>? UserPermissions { get; set; }
    }
}
