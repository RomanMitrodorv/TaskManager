using MongoDB.Bson;
using Telegram.API.Repository;
using Telegram.API.Service;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.API.Commands.List
{
    public class StopCommand : Command
    {
        private const string STOP_BOT_MESSAGE = "Уведомления больше приходить не будут!";

        public StopCommand(TelegramService telegram, IUserRepository userRepository) : base(telegram, userRepository)
        {
        }

        public async override Task ExecuteAsync(Update update)
        {
            var message = update.Message.Text;

            var userId = new ObjectId(message.Substring(message.LastIndexOf('/') - 1));

            var user = userRepository.GetAsync(userId);

            var chat = update.Message.Chat;

            if (user == null)
            {
                await userRepository.DeleteAsync(userId);

                await telegram._bot.SendTextMessageAsync(chat.Id, STOP_BOT_MESSAGE);
            }
            else
            {
                throw new Exception("You can't turn off notifications if you haven't created a user yet");
            }
        }
    }
}
