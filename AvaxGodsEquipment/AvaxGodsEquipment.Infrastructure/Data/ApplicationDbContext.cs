using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AvaxGodsEquipment.Core;
using AvaxGodsEquipment.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<Player> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.WalletAddress);
            entity.Property(p => p.Balance)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Price)
                .HasPrecision(18, 2);

            // Đảm bảo WalletAddress có thể null
            entity.Property(e => e.WalletAddress)
                .IsRequired(false);

            // Relationship với Player
            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(e => e.WalletAddress)
                .IsRequired(false);
        });
    }
}

