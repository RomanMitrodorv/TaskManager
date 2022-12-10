using Telegram.API.Commands;
using Telegram.API.Repository;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.API.Service
{
    public class TelegramService : ITelegramService
    {
        public TelegramBotClient _bot;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly CommandContainer _commandContainer;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;


        private const string COMMAND_PREFIX = "/";

        public TelegramService(IConfiguration configuration, IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;

            _userRepository = userRepository;

            _bot = new TelegramBotClient(configuration["TelegramToken"]);

            StartReceiving();

            _commandContainer = new CommandContainer(this, userRepository);
        }

        public Task StartReceiving()
        {
            _bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: new ReceiverOptions() { AllowedUpdates = new UpdateType[] { UpdateType.Message } },
                cancellationToken: _cancellationTokenSource.Token
            );

            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;

            if (messageText.StartsWith(COMMAND_PREFIX))
            {
                var commandIdentifier = messageText.Split("&")[0].ToLower();

                await _commandContainer.RetrieveCommand(commandIdentifier).ExecuteAsync(update);
            }
            else
            {
                await _commandContainer.RetrieveCommand(CommandList.No).ExecuteAsync(update);
            }

            var user = await _userRepository.GetByExternalIdAsync(update.Message.Chat.Id);

            if (user != null)
            {
                await _messageRepository.CreateAsync(new Model.Message()
                {
                    CreatedAt = DateTime.UtcNow,
                    Text = messageText,
                    UserId = user.Id
                });
            }
        }


        public async Task SendMessageAsync(Guid userId, string message)
        {
            var user = await _userRepository.GetByIdentityIdAsync(userId);
            if (user != null)
            {
                await _bot.SendTextMessageAsync(user.ExternalId, message);
            }

            throw new Exception("User not foud");
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

    }
}
