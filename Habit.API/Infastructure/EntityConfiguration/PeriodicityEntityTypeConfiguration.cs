using Habit.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Habit.API.Infastructure.EntityConfiguration
{
    public class PeriodicityEntityTypeConfiguration : IEntityTypeConfiguration<Periodicity>
    {
        public void Configure(EntityTypeBuilder<Periodicity> builder)
        {
            builder.ToTable("Periodicity");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseHiLo("periodicity_hilo").IsRequired();

            builder.Property(x => x.Code).IsRequired().HasMaxLength(64);

            builder.HasIndex(x => x.Code).IsUnique();

            builder.HasData(new Periodicity()
            {
                Id = 1,
                Code = "everyday",
                Name = "Каждый день"
            });
        }
    }
}
