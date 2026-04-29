using ApiBabterStyle.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiBabterStyle.Data;

public class BarberShopDbContext(DbContextOptions<BarberShopDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Barber> Barbers => Set<Barber>();
    public DbSet<BarberService> Services => Set<BarberService>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasIndex(appointment => new { appointment.BarberId, appointment.ScheduledAt });

        modelBuilder.Entity<BarberService>()
            .Property(service => service.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Product>()
            .Property(product => product.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Product>()
            .HasIndex(product => product.Name);

        modelBuilder.Entity<Barber>().HasData(
            new Barber
            {
                Id = Guid.Parse("0d27f419-3cde-458f-90d1-1a2f9905a301"),
                Name = "Carlos Silva",
                Specialty = "Corte masculino e degradê"
            },
            new Barber
            {
                Id = Guid.Parse("8de280dd-f78c-4ed3-bb35-67820c03fb3d"),
                Name = "Rafael Santos",
                Specialty = "Barba, navalha e sobrancelha"
            });

        modelBuilder.Entity<BarberService>().HasData(
            new BarberService
            {
                Id = Guid.Parse("82e9cfcb-0709-4e62-b7ce-7150e1c63583"),
                Name = "Corte",
                Description = "Corte masculino completo",
                Price = 45m,
                DurationMinutes = 45
            },
            new BarberService
            {
                Id = Guid.Parse("fc39f0ec-a173-4f4c-9bf1-7d4ac4c64cd6"),
                Name = "Barba",
                Description = "Barba com toalha quente e acabamento na navalha",
                Price = 35m,
                DurationMinutes = 30
            },
            new BarberService
            {
                Id = Guid.Parse("f89df8e5-84f4-491e-8edb-80105a9502a5"),
                Name = "Corte + Barba",
                Description = "Pacote completo de corte e barba",
                Price = 75m,
                DurationMinutes = 75
            });
    }
}
