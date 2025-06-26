using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Domain.Entities
{
    public class UserPermission
    {
        [Key]
        public int? UserPermissionId { get; set; }
        public string PageUrl { get; set; }
        
        [ForeignKey("Users")]
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CreatedUser")]
        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UpdatedUser")]
        public int? UpdatedBy { get; set; }

        //Navigation Property

        [JsonIgnore]
        public virtual User? Users { get; set; }
    }
}
