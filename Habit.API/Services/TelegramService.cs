using GrpcTelegram;

namespace Habit.API.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly Telegram.TelegramClient _telegramClient;
        private readonly ILogger<TelegramService> _logger;

        public TelegramService(Telegram.TelegramClient telegramClient, ILogger<TelegramService> logger)
        {
            _telegramClient = telegramClient;
            _logger = logger;
        }

        public async Task SendMessageAsync(Guid userId, string message)
        {
            _logger.LogDebug("grpc client created, user = {@id}, message = {@message}", userId.ToString(), message);

            var response = await _telegramClient.SendMessageAsync(new TelegramRequest()
            {
                Message = message,
                UserId = userId.ToString(),
            });

            _logger.LogDebug("grpc response {@response}", response);
        }
    }
}
