using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
public class EquipmentService : IEquipmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IPlayerService _playerService;
    private readonly ILogger<EquipmentService> _logger; // Thêm logger field

    public EquipmentService(ApplicationDbContext context, IPlayerService playerService, ILogger<EquipmentService> logger) 
    {
        _context = context;
        _playerService = playerService;
        _logger = logger; // Gán logger
    }

    public async Task<Equipment> BuyEquipment(string walletAddress, int equipmentId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var equipment = await _context.Equipments
                .FirstOrDefaultAsync(e => e.Id == equipmentId && e.WalletAddress == null);

            if (equipment == null)
                throw new InvalidOperationException("Equipment not found or already owned");

            var player = await _playerService.GetPlayer(walletAddress);
            if (player == null)
                throw new InvalidOperationException("Player not found");

            if (player.Balance < equipment.Price)
                throw new InvalidOperationException("Insufficient balance");

            // Update player balance
            await _playerService.UpdateBalance(walletAddress, -equipment.Price);

            // Assign equipment to player
            equipment.WalletAddress = walletAddress;
            equipment.CreatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return equipment;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Equipment> UpgradeEquipment(string walletAddress, int equipmentId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var equipment = await _context.Equipments
                .FirstOrDefaultAsync(e => e.Id == equipmentId && e.WalletAddress == walletAddress);

            if (equipment == null)
                throw new InvalidOperationException("Equipment not found or not owned by player");

            decimal upgradeCost = equipment.Level * 0.1m; // Cost increases with level
            
            var player = await _playerService.GetPlayer(walletAddress);
            if (player.Balance < upgradeCost)
                throw new InvalidOperationException("Insufficient balance");

            // Update player balance
            await _playerService.UpdateBalance(walletAddress, -upgradeCost);

            // Upgrade equipment
            equipment.Level++;
            equipment.Attack += 2;
            equipment.Defense += 2;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return equipment;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Equipment>> GetPlayerEquipments(string walletAddress)
    {
        return await _context.Equipments
            .Where(e => e.WalletAddress == walletAddress)
            .ToListAsync();
    }

    public async Task AddReward(string walletAddress, decimal amount)
    {
        await _playerService.UpdateBalance(walletAddress, amount);
    }

    public async Task<IEnumerable<Equipment>> GetShopEquipments()
    {
        try
        {
            var equipments = await _context.Equipments
                .Where(e => e.WalletAddress == null)
                .ToListAsync();

            _logger.LogInformation($"Found {equipments.Count()} available equipments in shop");
            return equipments;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting shop equipments: {ex.Message}");
            throw;
        }
    }

}