namespace Telegram.API.Model;

public class User : BaseMongoEntity
{
    public string IdentityId { get; set; }
    public long ExternalId { get; set; }
    public string Name { get; set; }
    public string? Phone { get; set; }
    public string? UserName { get; set; }
}