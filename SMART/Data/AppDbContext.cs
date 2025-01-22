using Microsoft.EntityFrameworkCore;
using SMART.Models;

namespace SMART.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<EquipmentPlacementContract> EquipmentPlacementContract { get; set; }
        public DbSet<ProcessEquipmentType> ProcessEquipmentType { get; set; }   
        public DbSet<ProductionFacility> ProductionFacility { get; set; }
    }
}
