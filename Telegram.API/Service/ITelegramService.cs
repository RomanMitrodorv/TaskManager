namespace Telegram.API.Service;

public interface ITelegramService
{
    Task SendMessageAsync(Guid userId, string message);
    Task StartReceiving();
}