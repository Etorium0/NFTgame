public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Level { get; set; }
    public string? WalletAddress { get; set; }
    public bool IsEquipped { get; set; }
    public DateTime CreatedAt { get; set; }
}