namespace Habit.API.Services
{
    public class IdentityService : IIdentityService
    {

        private IHttpContextAccessor _httpContextAccessor;

        public IdentityService(IHttpContextAccessor context)
        {
            _httpContextAccessor = context ?? throw new ArgumentNullException(nameof(context));
        }
        public string GetUserIdentity() => _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;

        public string GetUserName() => _httpContextAccessor.HttpContext.User.Identity.Name;
    }
}
