using MongoDB.Bson;
using MongoDB.Driver;
using Telegram.API.Model;

namespace Telegram.API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoClient client)
        {
            var database = client.GetDatabase("TelegramDatabase");

            var collection = database.GetCollection<User>(nameof(User));

            _users = collection;
        }


        public async Task<ObjectId> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user.Id;
        }

        public async Task<bool> DeleteAsync(ObjectId objectId)
        {
            var user = Builders<User>.Filter.Eq(c => c.Id, objectId);

            var result = await _users.DeleteOneAsync(user);

            return result.DeletedCount == 1;
        }

        public async Task<User> GetAsync(ObjectId objectId)
        {
            var user = Builders<User>.Filter.Eq(c => c.Id, objectId);
            var result = await _users.FindAsync(user);

            return await result.FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<User>> GetAsync()
        {
            var user = await _users.Find(_ => true).ToListAsync();

            return user;
        }

        public async Task<IEnumerable<User>> GetByNameAsync(string name)
        {
            var user = Builders<User>.Filter.Eq(c => c.UserName, name);

            var result = await _users.FindAsync(user);

            return await result.ToListAsync();

        }

        public async Task<User> GetByExternalIdAsync(long id)
        {
            var user = Builders<User>.Filter.Eq(c => c.ExternalId, id);

            var result = await _users.FindAsync(user);

            return await result.FirstOrDefaultAsync();

        }

        public async Task<User> GetByIdentityIdAsync(Guid id)
        {
            var user = Builders<User>.Filter.Eq(c => c.IdentityId, id.ToString());

            var result = await _users.FindAsync(user);

            return await result.FirstOrDefaultAsync();
        }
    }
}
