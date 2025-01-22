using System.ComponentModel.DataAnnotations;

namespace SMART.Models
{
    public class EquipmentPlacementContract
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EquipmentTypeId { get; set; }
        [Required]
        public ProcessEquipmentType EquipmentType { get; set; }
        [Required]
        public int ProductionFacilityId { get; set; }
        [Required]
        public ProductionFacility ProductionFacility { get; set; }
        [Required]
        public int NumberOfUnits { get; set; }
    }
}
