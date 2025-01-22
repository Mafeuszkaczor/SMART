using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMART.Data;
using SMART.DTOs;
using SMART.Models;

namespace SMART.Services
{
    public class EquipmentService : IEquipmentService
    {
        private AppDbContext _context;

        public EquipmentService(AppDbContext context)
        {
            _context = context;
        }

        public List<EquipmentPlacementContractDTO> GetAllContracts()
        {
            var Contracts = _context.EquipmentPlacementContract
                .Include(c => c.ProductionFacility)
                .Include(c => c.EquipmentType).ToList();
            
            if(Contracts == null)
            {
                return null;
            }

            List<EquipmentPlacementContractDTO> contracts = new List<EquipmentPlacementContractDTO>();

            foreach(var item in Contracts)
            {
                contracts.Add(new EquipmentPlacementContractDTO(){
                    ProductionFacilityName = item.ProductionFacility.Name,
                    EquipmentTypeName = item.EquipmentType.Name,
                    Quantity = item.NumberOfUnits
                });
            }

            var result = contracts.GroupBy( g => new {g.ProductionFacilityName, g.EquipmentTypeName})
                            .Select( s => new EquipmentPlacementContractDTO{
                                ProductionFacilityName = s.Key.ProductionFacilityName,
                                EquipmentTypeName = s.Key.EquipmentTypeName,
                                Quantity = s.Sum(x => x.Quantity)})
                            .OrderBy(o => o.ProductionFacilityName)
                            .ThenBy(t => t.EquipmentTypeName)
                            .ToList();

            return result;
        }

        public async Task<ActionResult<string>> CreateContract(NewEquipmentPlacementContractDTO contractDto)
        {
            if (contractDto == null)
            {
                return new BadRequestObjectResult("Contract data cannot be null.");
            }

            if(contractDto.NumberOfUnits <= 0)
            {
                return new BadRequestObjectResult("Number of units can't be 0 or less.");
            }

            var productionFacility = _context.ProductionFacility
                .FirstOrDefault(p => p.Code == contractDto.ProductionFacilityCode);
            if (productionFacility == null)
            {
                return new NotFoundObjectResult("Production facility not found.");
            }

            var equipmentType = _context.ProcessEquipmentType
                .FirstOrDefault(e => e.Code == contractDto.EquipmentTypeCode);
            if (equipmentType == null)
            {
                return new NotFoundObjectResult("Equipment type not found.");
            }

            if (productionFacility.StandardArea <contractDto.NumberOfUnits * equipmentType.Area)
            {
                return new BadRequestObjectResult("Not enough space in the production facility.");
            }

            if (!IsSpaceAvailable(contractDto))
            {
                return new BadRequestObjectResult("No left space in the production facility.");
            }

            var contract = new EquipmentPlacementContract
            {
                ProductionFacilityId = _context.ProductionFacility.Where(c => c.Code == contractDto.ProductionFacilityCode).Select(c => c.Id).FirstOrDefault(),
                EquipmentTypeId = _context.ProcessEquipmentType.Where(c => c.Code == contractDto.EquipmentTypeCode).Select(c => c.Id).FirstOrDefault(),
                NumberOfUnits = contractDto.NumberOfUnits
            };

            _context.EquipmentPlacementContract.Add(contract);
            _context.SaveChanges();
            
            return new OkObjectResult("Contract created!");
        }

        private bool IsSpaceAvailable(NewEquipmentPlacementContractDTO contract)
        {
            var EquipmentArea = _context.ProcessEquipmentType
                .Where(c => c.Code == contract.EquipmentTypeCode)
                .Sum(c => c.Area) * contract.NumberOfUnits;

            var FacilityArea = _context.ProductionFacility
                .Where(c => c.Code == contract.ProductionFacilityCode)
                .Sum(c => c.StandardArea);

            int OccupiedArea = 0;
            var Equipments = _context.ProcessEquipmentType.ToList();

            foreach(var equipment in Equipments)
            {
                OccupiedArea += _context.EquipmentPlacementContract
                    .Where(c => c.ProductionFacility.Code == contract.ProductionFacilityCode) //idk bo tu moze byc null .code
                    .Where(c => c.EquipmentTypeId == equipment.Id)
                    .Sum(c => c.NumberOfUnits) * equipment.Area;
            }

            if (OccupiedArea + EquipmentArea > FacilityArea)
            {
                return false;
            }

            return true;
        }
    }
}
