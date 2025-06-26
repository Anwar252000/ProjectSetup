using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public int? UserRoleId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Navigation Property
        public virtual UserRole? UserRoles { get; set; }
        public virtual ICollection<UserPermission>? UserPermissions { get; set; }
    }
}
