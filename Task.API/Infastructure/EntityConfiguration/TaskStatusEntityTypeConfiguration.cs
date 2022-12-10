namespace Task.API.Infastructure.EntityConfiguration
{
    public class TaskStatusEntityTypeConfiguration : IEntityTypeConfiguration<Model.TaskStatus>
    {
        public void Configure(EntityTypeBuilder<Model.TaskStatus> builder)
        {
            builder.ToTable("TaskStatus");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseHiLo("task_status_hilo").IsRequired();

            builder.Property(x => x.Code).IsRequired().HasMaxLength(64);

            builder.HasIndex(x => x.Code).IsUnique();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

            builder.HasData(
                new { Id = 3, Code = "created", Name = "Создано", SortOrder = 10 },
                new { Id = 1, Code = "doing", Name = "В работе", SortOrder = 20 },
                new { Id = 4, Code = "completed", Name = "Выполнено", SortOrder = 30 });
        }
    }
}
