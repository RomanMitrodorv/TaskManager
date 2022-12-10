using Task.API.Infastructure.EntityConfiguration;
using TaskStatus = Task.API.Model.TaskStatus;

namespace Task.API.Infastructure
{
    public class TaskContext : DbContext
    {
        public DbSet<ScheduledTask> Tasks { get; set; }
        public DbSet<TaskStatus> TaskStatus { get; set; }
        public DbSet<TaskLabel> TaskLabel { get; set; }
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TaskStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduledTaskEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TaskLabelEntityTypeConfiguration());
        }
    }
}
