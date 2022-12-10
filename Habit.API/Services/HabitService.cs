using Habit.API.Infastructure;
using Microsoft.EntityFrameworkCore;

namespace Habit.API.Services
{
    public class HabitService : IHabitService
    {
        private readonly HabitContext _habitContext;
        public HabitService(HabitContext habitContext)
        {
            _habitContext = habitContext ?? throw new ArgumentNullException(nameof(habitContext));
        }

        public async Task ResetHabits()
        {
            using var transaction = await _habitContext.Database.BeginTransactionAsync();
            try
            {
                _habitContext.Habits.ToList().ForEach(habit => { habit.CompletedCount = 0; });
                await _habitContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }
        }

    }
}
