namespace Task.API.Infastructure.EntityConfiguration
{
    public class ScheduledTaskEntityTypeConfiguration : IEntityTypeConfiguration<ScheduledTask>
    {
        public void Configure(EntityTypeBuilder<ScheduledTask> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.Date).IsRequired();

            builder.Property(x => x.Id).UseHiLo("scheduled_task_hilo").IsRequired();

            builder.HasOne(x => x.Label).WithMany().HasForeignKey(q => q.LabelId);
        }
    }
}
