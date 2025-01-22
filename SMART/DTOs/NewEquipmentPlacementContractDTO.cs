using System.ComponentModel.DataAnnotations;

namespace SMART.DTOs
{
    public class NewEquipmentPlacementContractDTO
    {
        public required string ProductionFacilityCode { get; set; }
        public required string EquipmentTypeCode { get; set; }
        public required int NumberOfUnits { get; set; }
    }
}
