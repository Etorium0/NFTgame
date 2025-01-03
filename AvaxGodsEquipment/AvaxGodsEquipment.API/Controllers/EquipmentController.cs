using Microsoft.AspNetCore.Mvc;
using AvaxGodsEquipment.Core;
using System.Threading.Tasks;
using System.Data.SqlTypes;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;
    private readonly ILogger<EquipmentController> _logger;

    public EquipmentController(IEquipmentService equipmentService, ILogger<EquipmentController> logger)
    {
        _equipmentService = equipmentService;
        _logger = logger;
    }

    [HttpGet("{walletAddress}")]
    public async Task<ActionResult<IEnumerable<Equipment>>> GetPlayerEquipments(string walletAddress)
    {
        try
        {
            var equipments = await _equipmentService.GetPlayerEquipments(walletAddress);
            return Ok(equipments);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting player equipments: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("buy")]
    public async Task<ActionResult<Equipment>> BuyEquipment([FromBody] BuyEquipmentRequest request)
    {
        try
        {
            var equipment = await _equipmentService.BuyEquipment(request.WalletAddress, request.EquipmentId);
            return Ok(equipment);
        }
        catch (Exception ex) when (ex is NotFoundException || ex is InsufficientFundsException)
        {
            _logger.LogWarning($"Buy equipment failed: {ex.Message}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error in buy equipment: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("upgrade")]
    public async Task<ActionResult<Equipment>> UpgradeEquipment([FromBody] UpgradeEquipmentRequest request)
    {
        try
        {
            var equipment = await _equipmentService.UpgradeEquipment(request.WalletAddress, request.EquipmentId);
            return Ok(equipment);
        }
        catch (Exception ex) when (ex is NotFoundException || ex is InsufficientFundsException)
        {
            _logger.LogWarning($"Upgrade equipment failed: {ex.Message}");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error in upgrade equipment: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipments()
    {
        try
        {
            var equipments = await _equipmentService.GetShopEquipments();
            return Ok(equipments);
        }
        catch (SqlNullValueException ex)
        {
            _logger.LogError($"SQL Null Value Exception: {ex.Message}");
            return StatusCode(500, "Database error: Null value found where not allowed");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting equipments: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
