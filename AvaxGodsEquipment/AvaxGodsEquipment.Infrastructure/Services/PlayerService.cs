using Microsoft.EntityFrameworkCore;

public class PlayerService : IPlayerService
{
    private readonly ApplicationDbContext _context;

    public PlayerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Player> GetPlayer(string walletAddress)
    {
        var player = await _context.Players
            .Include(p => p.Equipments)
            .FirstOrDefaultAsync(p => p.WalletAddress == walletAddress);

        if (player == null)
        {
            // Auto-create new player if not exists
            player = new Player
            {
                WalletAddress = walletAddress,
                Balance = 0,
                Equipments = new List<Equipment>()
            };
            
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }

        return player;
    }

    public async Task<decimal> GetBalance(string walletAddress)
    {
        var player = await GetPlayer(walletAddress);
        return player.Balance;
    }

    public async Task UpdateBalance(string walletAddress, decimal amount)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var player = await _context.Players
                .FirstOrDefaultAsync(p => p.WalletAddress == walletAddress);

            if (player == null)
                throw new InvalidOperationException("Player not found");

            if (player.Balance + amount < 0)
                throw new InvalidOperationException("Insufficient balance");

            player.Balance += amount;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

// AvaxGodsEquipment.Infrastructure/Services/Exceptions/InsufficientBalanceException.cs
public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException() : base("Insufficient balance")
    {
    }

    public InsufficientBalanceException(string message) : base(message)
    {
    }
}