using System.ComponentModel.DataAnnotations;

namespace SMART.Models
{
    public class ProcessEquipmentType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Area { get; set; }
    }
}
