using System.ComponentModel.DataAnnotations;

namespace Backend.Domain.Entities
{
    public class UserRole
    {
        [Key]
        public int? UserRoleId { get; set; }
        public string RoleName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
