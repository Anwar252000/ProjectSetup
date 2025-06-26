using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Domain.Entities
{
    public class VehicleColor
    {
        [Key]
        public int VehicleColorId { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("CreatedUser")]
        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UpdatedUser")]
        public int? UpdatedBy { get; set; }

        //Navigation Property
        public virtual User? CreatedUser { get; set; }
        public virtual User? UpdatedUser { get; set; }
    }
}
