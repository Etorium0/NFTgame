public interface IEquipmentService
{
    Task<Equipment> BuyEquipment(string walletAddress, int equipmentId);
    Task<Equipment> UpgradeEquipment(string walletAddress, int equipmentId);
    Task<IEnumerable<Equipment>> GetPlayerEquipments(string walletAddress);
    Task<IEnumerable<Equipment>> GetShopEquipments();
    Task AddReward(string walletAddress, decimal amount);
}