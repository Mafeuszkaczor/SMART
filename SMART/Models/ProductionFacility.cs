﻿using System.ComponentModel.DataAnnotations;

namespace SMART.Models
{
    public class ProductionFacility
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int StandardArea { get; set; }

    }
}
