using Microsoft.AspNetCore.Mvc;
using SMART.DTOs;

namespace SMART.Services
{
    public interface IEquipmentService
    {
        public List<EquipmentPlacementContractDTO> GetAllContracts();
        public Task<ActionResult<string>> CreateContract(NewEquipmentPlacementContractDTO contractDTO);
    }
}
