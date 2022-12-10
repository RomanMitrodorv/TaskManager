namespace Services.Identity.API.Configuration
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("task", "Task Service"),
                new ApiResource("habit", "Habit Service"),

            };
        }
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "taskswaggerui",
                    ClientName = "Task Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["TaskApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["TaskApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "task"
                    }
                },
                new Client
                {
                    ClientId = "habitswaggerui",
                    ClientName = "Habit Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["HabitApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["HabitApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "habit"
                    }
                },
                new Client
                {
                    ClientId = "ClientApi",
                    RequireClientSecret = false,
                    AllowedGrantTypes = { GrantType.ResourceOwnerPassword },
                    AllowedScopes = { "task", "habit" },
                    AllowOfflineAccess = true,
                },
            };
        }
    }
}