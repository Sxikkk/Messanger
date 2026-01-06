using Microsoft.EntityFrameworkCore;

namespace Persistance;

public class DatabaseCreator
{
    public static async Task InitializeDatabaseAsync(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyAppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        await using var context = new MyAppDbContext(optionsBuilder.Options);
        
        var canConnect = await context.Database.CanConnectAsync();
        
        if (!canConnect)
        {
            var created = await context.Database.EnsureCreatedAsync();
            
            if (created)
            {
                Console.WriteLine("База данных создана.");
            }
        }
        else
        {
            Console.WriteLine("База данных уже существует.");
        }
    }
}