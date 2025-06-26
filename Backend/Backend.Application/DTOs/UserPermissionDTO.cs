using Backend.Domain.Entities;

namespace Backend.Application.DTOs
{
    public class UserPermissionDTO
    {
        public int? UserPermissionId { get; set; }
        public List<string> PageUrl { get; set; }
        public int UserId { get; set; }

        public string? UserName { get; set; } 

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

    }
}
