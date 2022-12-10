using Habit.API.Infastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Habit.API.Factories
{
    public class HabitFactoryDesignFactory : IDesignTimeDbContextFactory<HabitContext>
    {
        public HabitContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<HabitContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], sqlServerOptionsAction: o => o.MigrationsAssembly("Habit.API"));

            return new HabitContext(optionsBuilder.Options);
        }
    }
}
