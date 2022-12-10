namespace Habit.API.Services;

public interface ITelegramService
{
    Task SendMessageAsync(Guid userId, string message);
}