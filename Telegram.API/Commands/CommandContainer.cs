using Telegram.API.Commands.List;
using Telegram.API.Repository;
using Telegram.API.Service;

namespace Telegram.API.Commands
{
    public class CommandContainer
    {
        private readonly Dictionary<string, Command> _commands;
        private readonly UnknownCommand _unknownCommand;

        public CommandContainer(TelegramService telegram, IUserRepository userRepository)
        {
            _commands = new Dictionary<string, Command>()
            {
                { CommandList.Start, new StartCommand(telegram, userRepository) },
                { CommandList.Stop, new StopCommand(telegram, userRepository)},
                { CommandList.No, new NoCommand(telegram, userRepository)}
            };

            _unknownCommand = new UnknownCommand(telegram, userRepository);
        }

        public Command RetrieveCommand(string commandId)
        {
            return _commands.GetValueOrDefault(commandId, _unknownCommand);
        }
    }
}
