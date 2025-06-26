using Backend.Domain.Entities;

namespace Backend.Application.DTOs
{
    public class VehicleDetailDTO
    {
        public int VehicleDetailId { get; set; }

        public int? VehicleBookingId { get; set; }
        public int? VehiclePurchaseId { get; set; }

        public string Condition { get; set; }

        public string InvoicedOrAllocation { get; set; }

        public string Make { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string FuelType { get; set; }

        public string ColorType { get; set; }

        public int? VehicleColorId { get; set; }

        public string RegistrationType { get; set; }

        public int? MakeYear { get; set; }

        public string Engine { get; set; }

        public string Chassis { get; set; }

        public string Transmission { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        //Navigation Property
        public virtual VehicleColor? VehicleColors { get; set; }
        public virtual User? CreatedUser { get; set; }
        public virtual User? UpdatedUser { get; set; }
    }
}
