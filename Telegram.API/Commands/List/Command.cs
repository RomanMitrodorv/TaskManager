using Telegram.API.Repository;
using Telegram.API.Service;
using Telegram.Bot.Types;

namespace Telegram.API.Commands.List
{
    public abstract class Command
    {
        protected TelegramService telegram;
        protected IUserRepository userRepository;

        protected Command(TelegramService telegram, IUserRepository userRepository)
        {
            this.telegram = telegram;
            this.userRepository = userRepository;
        }

        public abstract Task ExecuteAsync(Update update);
    }
}

