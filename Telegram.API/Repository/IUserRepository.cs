using MongoDB.Bson;
using Telegram.API.Model;

namespace Telegram.API.Repository
{
    public interface IUserRepository
    {
        Task<ObjectId> CreateAsync(User user);
        Task<User> GetAsync(ObjectId objectId);
        Task<User> GetByIdentityIdAsync(Guid id);
        Task<IEnumerable<User>> GetAsync();
        Task<IEnumerable<User>> GetByNameAsync(string name);
        Task<User> GetByExternalIdAsync(long id);
        Task<bool> DeleteAsync(ObjectId objectId);
    }
}
