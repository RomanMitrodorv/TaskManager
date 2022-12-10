using MongoDB.Bson;
using MongoDB.Driver;
using Telegram.API.Model;


namespace Telegram.API.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMongoCollection<Message> _messages;

        public MessageRepository(IMongoClient client)
        {
            var database = client.GetDatabase("TelegramDatabase");

            var collection = database.GetCollection<Message>(nameof(Message));

            _messages = collection;
        }

        public async Task<ObjectId> CreateAsync(Message message)
        {
            await _messages.InsertOneAsync(message);
            return message.Id;
        }
    }
}