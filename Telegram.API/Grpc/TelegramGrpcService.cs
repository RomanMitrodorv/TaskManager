
using Grpc.Core;
using Telegram.API.Service;

namespace GrpcTelegram
{
    public class TelegramGrpcService : Telegram.TelegramBase
    {
        private readonly ITelegramService _telegramService;

        public TelegramGrpcService(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        public async override Task<Empty> SendMessage(TelegramRequest request, ServerCallContext context)
        {
            await _telegramService.SendMessageAsync(new Guid(request.UserId), request.Message);

            return new Empty();
        }
    }
}
