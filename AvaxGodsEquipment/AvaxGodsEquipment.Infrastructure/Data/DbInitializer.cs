public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        // Kiểm tra đã có equipment chưa
        if (context.Equipments.Any())
        {
            return;   // DB đã được seed
        }

        var equipments = new Equipment[]
        {
            new Equipment{
                Name="Sword of Power",
                Description="A powerful sword",
                Price=1.0m,
                Attack=5,
                Defense=2,
                Level=1
            },
            new Equipment{
                Name="Shield of Protection",
                Description="A sturdy shield",
                Price=0.8m,
                Attack=1,
                Defense=6,
                Level=1
            },
            // Thêm các equipment khác...
        };

        foreach (Equipment e in equipments)
        {
            context.Equipments.Add(e);
        }
        context.SaveChanges();
    }
}