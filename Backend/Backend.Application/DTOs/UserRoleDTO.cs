namespace Backend.Application.DTOs
{
    public class UserRoleDTO
    {
        public int? UserRoleId { get; set; }
        public string RoleName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
