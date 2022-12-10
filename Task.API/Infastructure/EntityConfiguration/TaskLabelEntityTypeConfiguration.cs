namespace Task.API.Infastructure.EntityConfiguration
{
    public class TaskLabelEntityTypeConfiguration : IEntityTypeConfiguration<TaskLabel>
    {
        public void Configure(EntityTypeBuilder<TaskLabel> builder)
        {
            builder.ToTable("TaskLabel");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseHiLo("task_label_hilo").IsRequired();

            builder.Property(x => x.Code).IsRequired().HasMaxLength(64);

            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

            builder.HasData(
                new { Id = 1, Code = "Work", Name = "Работа" },
                new { Id = 2, Code = "Home", Name = "Дома" });
        }
    }
}
