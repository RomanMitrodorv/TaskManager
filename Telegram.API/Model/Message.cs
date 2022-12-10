using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Telegram.API.Model;

public class Message : BaseMongoEntity
{
    public string Text { get; set; }
    public ObjectId UserId { get; set; }
    [BsonIgnore]
    public User User { get; set; }
}

