using MongoDB.Bson;

namespace Telegram.API.Model
{
    public class BaseMongoEntity
    {
        public ObjectId Id { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
