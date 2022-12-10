using Habit.API.Infastructure;
using Habit.API.Models;
using Habit.API.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Habit.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
[Authorize]
public class HabitController : Controller
{
    private readonly HabitContext _habitContext;
    private readonly IIdentityService _identityService;
    private readonly ILogger<HabitController> _logger;
    public HabitController(HabitContext habitContext, IIdentityService identityService, ILogger<HabitController> logger, ITelegramService notificationService)
    {
        _habitContext = habitContext ?? throw new ArgumentNullException(nameof(habitContext));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("habit")]
    [ProducesResponseType(typeof(List<HabitModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<List<HabitModel>>> HabitByUserIdAsync()
    {
        var userId = Guid.Parse(_identityService.GetUserIdentity());

        var habit = await _habitContext.Habits.Where(x => x.UserId == userId)
                                .Include(x => x.Periodicity)
                                .Include(x => x.Notifications).ToListAsync();

        if (!habit.Any())
            return NotFound();

        return habit;
    }



    [HttpPut]
    [Route("habit")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> UpdateHabitAsync([FromBody] HabitModel habitForUpdate)
    {
        var userId = Guid.Parse(_identityService.GetUserIdentity());

        var habit = await _habitContext.Habits.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == habitForUpdate.Id && x.UserId == userId);

        if (habit == null)
            return NotFound(new { Messsage = $"Habit with id {habit.Id} not found" });

        //Это работает не правильно. Поправить
        //habit.Notifications = habitForUpdate.Notifications;
        //habit.Periodicity = habitForUpdate.Periodicity;
        //Это работает не правильно. Поправить


        if (habitForUpdate.CompletedCount <= habit.Count)
            habit.CompletedCount += 1;

        habit.Name = habitForUpdate.Name;

        _habitContext.Habits.Update(habit);

        await _habitContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteHabitAsync(int id)
    {
        var userId = Guid.Parse(_identityService.GetUserIdentity());

        var habit = await _habitContext.Habits.Include(u => u.Notifications)
            .SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (habit == null)
            return NotFound();

        habit.Notifications.ToList()
            .ForEach(x =>
            {
                if(x.JobName != null)
                    RecurringJob.RemoveIfExists(x.JobName);
            });

        _habitContext.Habits.Remove(habit);

        await _habitContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    [Route("habit")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> CreateHabitAsync([FromBody] HabitRequest inputHabit)
    {
        var userId = Guid.Parse(_identityService.GetUserIdentity());

        var habitExists = await _habitContext.Habits.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == inputHabit.Name && x.UserId == userId);

        if (habitExists != null)
            return Conflict(new { Messsage = $"Habit with id {habitExists.Id} is exists" });

        var habit = new HabitModel()
        {
            DateCreation = DateTime.Now,
            Name = inputHabit.Name,
            Count = inputHabit.Count,
            UserId = userId,
            PeriodicityId = inputHabit.PeriodicityId
        };

        await _habitContext.Habits.AddAsync(habit);

        await _habitContext.SaveChangesAsync();

        int i = 0;

        inputHabit.Notifications.ToList().ForEach(async x =>
        {
            i++;

            var jobName = $"{userId}-{habit.Name}-{i}";

            await _habitContext.Notifications.AddAsync(new Notification()
            {
                HabitId = habit.Id,
                Time = x.Time,
                JobName = jobName
            });

            RecurringJob.AddOrUpdate<ITelegramService>(jobName,
                                     (x) => x.SendMessageAsync(userId, $"Пора выполнять привычку {habit.Name}"),
                                     $"{x.Time.Minutes} {x.Time.Hours} * * *",
                                     TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        });

        await _habitContext.SaveChangesAsync();

        return Ok();
    }
}