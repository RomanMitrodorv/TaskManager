using MongoDB.Bson;
using Telegram.API.Model;

namespace Telegram.API.Repository
{
    public interface IMessageRepository
    {
        Task<ObjectId> CreateAsync(Message message);
    }
}
