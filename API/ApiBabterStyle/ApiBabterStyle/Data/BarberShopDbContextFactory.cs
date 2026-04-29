using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ApiBabterStyle.Data;

public class BarberShopDbContextFactory : IDesignTimeDbContextFactory<BarberShopDbContext>
{
    public BarberShopDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=localhost;Database=barberstyle;Username=postgres;Password=postgres";
        }

        var options = new DbContextOptionsBuilder<BarberShopDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new BarberShopDbContext(options);
    }
}
