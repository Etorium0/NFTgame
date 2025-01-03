public interface IPlayerService
{
    Task<Player> GetPlayer(string walletAddress);
    Task<decimal> GetBalance(string walletAddress);
    Task UpdateBalance(string walletAddress, decimal amount);
}