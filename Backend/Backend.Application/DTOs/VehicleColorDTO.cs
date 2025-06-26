using Backend.Domain.Entities;

namespace Backend.Application.DTOs
{
    public class VehicleColorDTO
    {
        public int VehicleColorId { get; set; }
        public string Color { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        //Navigation Property
        public virtual User? CreatedUser { get; set; }
        public virtual User? UpdatedUser { get; set; }
    }
}
