using System.ComponentModel.DataAnnotations;

namespace SMART.DTOs
{
    public class EquipmentPlacementContractDTO
    {
        public required string ProductionFacilityName { get; set; }
        public required string EquipmentTypeName { get; set; }
        public required int Quantity { get; set; }
    }
}
