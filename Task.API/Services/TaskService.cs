using Polly;
using System;
using System.Threading.Tasks;
using Task.API.Infastructure;

namespace Task.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskContext _taskContext;
        public TaskService(TaskContext taskContext)
        {
            _taskContext = taskContext ?? throw new ArgumentNullException(nameof(taskContext));
        }

        public async System.Threading.Tasks.Task DeactivateOldTasks()
        {
            using var transaction = await _taskContext.Database.BeginTransactionAsync();

            try
            {
                _taskContext.Tasks
                    .Include(x => x.Status)
                    .Where(q => q.Status.Code == "Completed")
                    .ToList()
                    .ForEach(task => { task.IsActive = false; });

                await _taskContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }
        }
    }
}
