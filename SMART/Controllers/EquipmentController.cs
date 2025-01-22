using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMART.Data;
using SMART.Models;
using SMART.DTOs;
using SMART.Services;

namespace SMART.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly ILogger<EquipmentController> _logger;
        private readonly AppDbContext _context;
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(ILogger<EquipmentController> logger, AppDbContext context, IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
            _context = context;
            _logger = logger;
        }

        [HttpGet("get-contracts")]
        public async Task<IActionResult> GetContracts()
        {
            var contracts = _equipmentService.GetAllContracts();
            if (contracts == null || !contracts.Any())
            {
                return NoContent();
            }
            return Ok(contracts); 
        }

        [HttpPost("create-contract/{ProductionFacilityCode}/{EquipmentTypeCode}/{NumberOfUnits}")]
        public async Task<ActionResult<string>> CreateContract([FromRoute]NewEquipmentPlacementContractDTO contractDto)
        {
            var result = await _equipmentService.CreateContract(contractDto);
            if(result.Result is OkObjectResult)
            {
                _ = Task.Run(async () =>
                    {
                    await Task.Delay(2000);
                    _logger.LogInformation("Contract created for facility: {ProductionFacilityCode} - {NumberOfUnits} units.", 
                        contractDto.ProductionFacilityCode, contractDto.NumberOfUnits);
                    }
                );
            }
            return result;
        }
    }
}
