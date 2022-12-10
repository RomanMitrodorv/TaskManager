using Telegram.API.Repository;
using Telegram.API.Service;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.API.Commands.List
{
    public class StartCommand : Command
    {

        private const string WELCOME_MESSAGE = "Вы зарегестрировались! Уведомления будут приходить о привычках";

        private const string ALREADY_REGISTRED_MESSAGE = "Вы зарегестрировались! Уведомления будут приходить о привычках";

        public StartCommand(TelegramService telegram, IUserRepository userRepository) : base(telegram, userRepository)
        {
        }

        public async override Task ExecuteAsync(Update update)
        {
            var message = update.Message.Text;

            var guid = message.Substring(message.LastIndexOf('&') + 1);

            Guid.TryParse(guid, out Guid userId);

            if (userId == Guid.Empty)
                return;

            var user = await userRepository.GetByIdentityIdAsync(userId);

            var chat = update.Message.Chat;

            if (user == null)
            {
                await userRepository.CreateAsync(new Model.User()
                {
                    CreatedAt = DateTime.Now,
                    UserName = chat.Username,
                    Name = $"{chat.FirstName} {chat.LastName}",
                    IdentityId = userId.ToString(),
                    ExternalId = chat.Id
                });

                await telegram._bot.SendTextMessageAsync(chat.Id, WELCOME_MESSAGE);
            }
            else
            {
                await telegram._bot.SendTextMessageAsync(chat.Id, ALREADY_REGISTRED_MESSAGE);
            }
        }
    }
}
