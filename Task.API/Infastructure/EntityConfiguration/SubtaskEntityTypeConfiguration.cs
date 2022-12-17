namespace Task.API.Infastructure.EntityConfiguration
{
    public class SubtaskEntityTypeConfiguration : IEntityTypeConfiguration<Subtask>
    {
        public void Configure(EntityTypeBuilder<Subtask> builder)
        {
            builder.ToTable("Subtask");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseHiLo("subtask_hilo").IsRequired();

            builder.HasOne(q => q.Task).WithMany(x => x.Subtasks).HasForeignKey(s => s.TaskId);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(516);
        }
    }
}
