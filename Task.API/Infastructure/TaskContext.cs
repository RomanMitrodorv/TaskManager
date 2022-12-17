using Task.API.Infastructure.EntityConfiguration;

namespace Task.API.Infastructure
{
    public class TaskContext : DbContext
    {
        public DbSet<ScheduledTask> Tasks { get; set; }
        public DbSet<Subtask> Subtasks { get; set; }
        public DbSet<TaskLabel> TaskLabel { get; set; }
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ScheduledTaskEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TaskLabelEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SubtaskEntityTypeConfiguration());
        }
    }
}
