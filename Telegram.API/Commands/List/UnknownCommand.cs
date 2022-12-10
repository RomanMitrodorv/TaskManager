using Telegram.API.Repository;
using Telegram.API.Service;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.API.Commands.List
{
    public class UnknownCommand : Command
    {
        private const string UNKNOWN_MESSAGE = "Я поддерживаю команды, начинающиеся со слеша(/).\n"
            + "Чтобы посмотреть список команд введите /help";


        public UnknownCommand(TelegramService telegram, IUserRepository userRepository) : base(telegram, userRepository)
        {
        }

        public async override Task ExecuteAsync(Update update)
        {
            var chat = update.Message.Chat;
            await telegram._bot.SendTextMessageAsync(chat.Id, UNKNOWN_MESSAGE);

        }
    }
}
