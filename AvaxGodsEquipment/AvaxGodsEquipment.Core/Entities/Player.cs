public class Player
{
    public string WalletAddress { get; set; }
    public decimal Balance { get; set; }
    public virtual ICollection<Equipment> Equipments { get; set; }
}