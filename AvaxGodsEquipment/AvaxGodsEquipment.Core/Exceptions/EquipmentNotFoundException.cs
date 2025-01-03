public class EquipmentNotFoundException : Exception
{
    public EquipmentNotFoundException(string message = "Equipment not found or already owned") 
        : base(message) { }
}