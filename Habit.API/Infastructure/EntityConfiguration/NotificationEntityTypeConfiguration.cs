using Habit.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habit.API.Infastructure.EntityConfiguration
{
    public class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseHiLo("notification_hilo").IsRequired();

            builder.Property(x => x.Time).IsRequired();

            builder.Property(x => x.HabitId).IsRequired();

        }
    }
}
