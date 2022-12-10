using Habit.API.Infastructure.EntityConfiguration;
using Habit.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Habit.API.Infastructure
{
    public class HabitContext : DbContext
    {
        public DbSet<HabitModel> Habits { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Periodicity> Periodicities { get; set; }
        public HabitContext(DbContextOptions<HabitContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new HabitEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PeriodicityEntityTypeConfiguration());
        }
    }
}
