using Xunit;
using SMART.DTOs;
using SMART.Models;
using SMART.Data;
using SMART.Controllers;
using SMART.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SMART.Tests;
public class EquipmentTest
{
    private readonly EquipmentService _services;
    private readonly AppDbContext _context;

    public EquipmentTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseInMemoryDatabase(databaseName: "TestDb")
                        .Options;

        _context = new AppDbContext(options);
        CreateDatabase();

        _services = new EquipmentService(_context);
    }

    private void CreateDatabase()
    {
        if (!_context.ProductionFacility.Any() && !_context.ProcessEquipmentType.Any())
        {
            var Facilities = new List<ProductionFacility>
            {
                new ProductionFacility { Code = "PROD01", Name = "Production 1", StandardArea = 100 },
                new ProductionFacility { Code = "PROD02", Name = "Production 2", StandardArea = 50 }
            };

            var Equipments = new List<ProcessEquipmentType>
            {
                new ProcessEquipmentType { Code = "EQ01", Name="Equipment 1", Area = 15 },
                new ProcessEquipmentType { Code = "EQ02", Name="Equipment 2", Area = 30 }
            };

            _context.ProductionFacility.AddRange(Facilities);
            _context.ProcessEquipmentType.AddRange(Equipments);
            _context.SaveChanges();
        }
    }

    [Fact]
    public async Task CreateContract_ReturnsBadRequest_WhenContractDataIsNull()
    {
        var expected = new BadRequestObjectResult("Contract data cannot be null.");
        NewEquipmentPlacementContractDTO contractDto = null;

        var result = await _services.CreateContract(contractDto);

        Assert.Equal(expected.StatusCode, ((BadRequestObjectResult)result.Result).StatusCode);
        Assert.Equal(expected.Value, ((BadRequestObjectResult)result.Result).Value);
    }

    [Fact]
    public async Task CreateContract_ReturnsNotFound_WhenContractFacilityCodeIsIncorrect()
    {
        var expected = new NotFoundObjectResult("Production facility not found.");
        var contractDto = new NewEquipmentPlacementContractDTO
        {
            ProductionFacilityCode = "000",
            EquipmentTypeCode = "EQ01",
            NumberOfUnits = 1
        };

        var result = await _services.CreateContract(contractDto);

        Assert.Equal(expected.StatusCode, ((NotFoundObjectResult)result.Result).StatusCode);
        Assert.Equal(expected.Value, ((NotFoundObjectResult)result.Result).Value);
    }

    [Fact]
    public async Task CreateContract_ReturnsBadRequest_WhenContractEquipmentCodeIsIncorrect()
    {
        var expected = new NotFoundObjectResult("Equipment type not found.");
        var contractDto = new NewEquipmentPlacementContractDTO
        {
            ProductionFacilityCode = "PROD01",
            EquipmentTypeCode = "000",
            NumberOfUnits = 10
        };

        var result = await _services.CreateContract(contractDto);

        Assert.Equal(expected.StatusCode, ((NotFoundObjectResult)result.Result).StatusCode);
        Assert.Equal(expected.Value, ((NotFoundObjectResult)result.Result).Value);
    }

    [Fact]
    public async Task CreateContract_ReturnsBadRequest_WhenContractNumberOfUnitIsZeroOrLess()
    {
        var expected = new BadRequestObjectResult("Number of units can't be 0 or less.");
        var contractDto = new NewEquipmentPlacementContractDTO
        {
            ProductionFacilityCode = "PROD01",
            EquipmentTypeCode = "EQ01",
            NumberOfUnits = 0
        };

        var result = await _services.CreateContract(contractDto);

        Assert.Equal(expected.StatusCode, ((BadRequestObjectResult)result.Result).StatusCode);
        Assert.Equal(expected.Value, ((BadRequestObjectResult)result.Result).Value);
    }

    [Fact]
    public async Task CreateContract_ReturnsBadRequest_WhenNotEnoughSpaceInProductionFacility()
    {
        var expected = new BadRequestObjectResult("Not enough space in the production facility.");
        var contractDto = new NewEquipmentPlacementContractDTO
        {
            ProductionFacilityCode = "PROD01",
            EquipmentTypeCode = "EQ01",
            NumberOfUnits = 10
        };

        var result = await _services.CreateContract(contractDto);

        Assert.Equal(expected.StatusCode, ((BadRequestObjectResult)result.Result).StatusCode);
        Assert.Equal(expected.Value, ((BadRequestObjectResult)result.Result).Value);
    }

    [Fact]
    public async Task CreateContract_ReturnsBadRequest_WhenNoLeftSpaceInProductionFacility()
    {
        var expected = new BadRequestObjectResult("No left space in the production facility.");
        var contractDto = new NewEquipmentPlacementContractDTO
        {
            ProductionFacilityCode = "PROD02",
            EquipmentTypeCode = "EQ02",
            NumberOfUnits = 1
        };

        var result = await _services.CreateContract(contractDto);
        result = await _services.CreateContract(contractDto);

        Assert.Equal(expected.StatusCode, ((BadRequestObjectResult)result.Result).StatusCode);
        Assert.Equal(expected.Value, ((BadRequestObjectResult)result.Result).Value);
    }

}