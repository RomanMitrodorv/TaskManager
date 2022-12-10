using Habit.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habit.API.Infastructure.EntityConfiguration
{
    public class HabitEntityTypeConfiguration : IEntityTypeConfiguration<HabitModel>
    {
        public void Configure(EntityTypeBuilder<HabitModel> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.DateCreation).IsRequired();

            builder.Property(x => x.Id).UseHiLo("habit_hilo").IsRequired(); ;

            builder.HasMany(x => x.Notifications)
                .WithOne(x => x.Habit)
                .HasForeignKey(q => q.HabitId)
                .IsRequired();

            builder.HasOne(x => x.Periodicity)
                   .WithMany()
                   .HasForeignKey(q => q.PeriodicityId);
        }
    }
}
