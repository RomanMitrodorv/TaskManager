using Microsoft.AspNetCore.Authorization;
using Task.API.Infastructure;
using Task.API.Services;

namespace Task.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly TaskContext _taskContext;
        private readonly IIdentityService _identityService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskContext taskContext, IIdentityService identityService, ILogger<TaskController> logger)
        {
            _taskContext = taskContext ?? throw new ArgumentNullException(nameof(taskContext));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("tasks")]
        [ProducesResponseType(typeof(List<ScheduledTask>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<ScheduledTask>>> TasksByUserIdAsync()
        {
            var userId = Guid.Parse(_identityService.GetUserIdentity());

            var tasks = await _taskContext.Tasks.Where(x => x.UserId == userId && x.IsActive == true)
                                                .Include(x => x.Label)
                                                .Include(x => x.Status)
                                                .OrderBy(x => x.ItemIndex)
                                                .ThenBy(x => x.Date)
                                                .ToListAsync();

            return tasks;
        }

        [HttpGet]
        [Route("{date}")]
        [ProducesResponseType(typeof(List<ScheduledTask>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<ScheduledTask>>> TasksByDateAsync(DateTime date)
        {
            var userId = Guid.Parse(_identityService.GetUserIdentity());
            var tasks = await _taskContext.Tasks.Where(x => x.UserId == userId && x.Date.Date == date.Date && x.IsActive == true)
                                                .OrderBy(x => x.ItemIndex)
                                                .ThenBy(x => x.Date)
                                                .ToListAsync();


            return tasks;
        }

        [HttpGet]
        [Route("taskstatus")]
        [ProducesResponseType(typeof(List<Model.TaskStatus>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<Model.TaskStatus>>> GetTaskStatusAsync()
        {
            return await _taskContext.TaskStatus.OrderBy(x => x.SortOrder).ToListAsync();
        }

        [HttpGet]
        [Route("tasklabel")]
        [ProducesResponseType(typeof(List<TaskLabel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TaskLabel>>> GetTaskLabelAsync()
        {
            return await _taskContext.TaskLabel.ToListAsync();
        }


        [HttpPut]
        [Route("tasks")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateTaskAsync([FromBody] ScheduledTask taskForUpdate)
        {
            var userId = Guid.Parse(_identityService.GetUserIdentity());

            var task = await _taskContext.Tasks.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == taskForUpdate.Id);

            if (task == null)
                return NotFound(new { Messsage = $"Task with id {taskForUpdate.Id} not found" });

            if (task.UserId != userId)
                return BadRequest(new { Messsage = "Various users" });

            task.Date = taskForUpdate.Date;
            task.LabelId = taskForUpdate.LabelId;
            task.StatusId = taskForUpdate.StatusId;
            task.Notes = taskForUpdate.Notes;
            task.Name = taskForUpdate.Name;
            task.ItemIndex = taskForUpdate.ItemIndex;

            _taskContext.Tasks.Update(task);

            await _taskContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult> DeleteTaskAsync(int id)
        {
            var userId = Guid.Parse(_identityService.GetUserIdentity());

            var task = await _taskContext.Tasks.SingleOrDefaultAsync(x => x.Id == id);

            if (task == null)
                return NotFound();

            if (task.UserId != userId)
                return BadRequest(new { Messsage = "Various users" });

            _taskContext.Tasks.Remove(task);

            await _taskContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("tasks")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> CreateTaskAsync([FromBody] ScheduledTask inputTask)
        {
            var userId = Guid.Parse(_identityService.GetUserIdentity());

            var initStatus = await _taskContext.TaskStatus.Where(x => x.Code == "created")
                                            .Select(x => x.Id)
                                            .FirstOrDefaultAsync();

            var maxItemIndex = await _taskContext.Tasks.Where(x => x.UserId == userId && x.StatusId == initStatus)
                                                 .OrderByDescending(x => x.ItemIndex)
                                                 .Select(x => x.ItemIndex)
                                                 .FirstOrDefaultAsync();

            var task = new ScheduledTask()
            {
                Date = inputTask.Date,
                IsActive = true,
                Name = inputTask.Name,
                Notes = inputTask.Notes,
                StatusId = initStatus,
                UserId = userId,
                LabelId = inputTask.LabelId,
                ItemIndex = maxItemIndex + 1
            };

            _taskContext.Tasks.Add(task);

            await _taskContext.SaveChangesAsync();

            return Ok();
        }
    }
}
