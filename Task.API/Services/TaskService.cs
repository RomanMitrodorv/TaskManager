using Polly;
using System;
using System.Threading.Tasks;
using Task.API.Infastructure;

namespace Task.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskContext _taskContext;
        private readonly ILogger<TaskService> _logger;

        public TaskService(TaskContext taskContext, ILogger<TaskService> logger)
        {
            _taskContext = taskContext ?? throw new ArgumentNullException(nameof(taskContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async System.Threading.Tasks.Task DeactivateOldTasks()
        {
            var executionStrategy = _taskContext.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _taskContext.Database.BeginTransactionAsync();

                try
                {
                    _taskContext.Tasks
                        .Where(q => q.Completed == false && q.Date < DateTime.Now)
                        .ToList()
                        .ForEach(task => { task.Date = DateTime.Now.Date; });

                    await _taskContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating completed tasks");
                    await transaction.RollbackAsync();
                }
            }
            );
        }
    }
}
