using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Customization;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Task.API.Infastructure;
using Task.API.Infrastructure.Filters;
using Task.API.Services;

namespace Task.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomDbContext(Configuration)
                .AddSwagger(Configuration)
                .AddHangfire(Configuration); 

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddTransient<ITaskService, TaskService>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            ConfigureAuthService(services);

            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);

            return new AutofacServiceProvider(containerBuilder.Build());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("USING PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }
            app.UseStaticFiles();
            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "TASK.API V1");
                    c.OAuthClientId("taskswaggerui");
                    c.OAuthAppName("Task Swagger UI");
                });

            var hangfire = Configuration.GetSection("HangfireCred").Get<HangfireOption>();

            var options = new DashboardOptions
            {
                Authorization = new[] {
                    new HangFireAuthorizationFilter(new[]
                    {
                        new HangfireUserCredentials
                        {
                            Username = hangfire.Username,
                            Password = hangfire.Password
                        }
                    })
                }
            };

            app.UseHangfireDashboard("/hangfire", options);

            app.UseRouting();
            app.UseCors("CorsPolicy");
            ConfigureAuth(app);
            app.UseEndpoints(endpoins =>
            {
                endpoins.MapDefaultControllerRoute();
                endpoins.MapControllers();
                endpoins.MapHangfireDashboard();
            });

            //Запускаем джоб на на каждый день
            RecurringJob.AddOrUpdate<ITaskService>(
                "DeactivateOldTasks.",
                (x) => x.DeactivateOldTasks(),
                "0 0 * * *",
                TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "task";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }

    public static class StartupExtensions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSerilogLogProvider()
                    .UseSqlServerStorage(Configuration["HangfireConnection"], new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

            services.AddHangfireServer();

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<TaskContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionString"], sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    sqlServerOptionsAction.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
            });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Task HTTP Api",
                    Version = "v1",
                    Description = "The Task Microservice HTTP Api"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                            Scopes = new Dictionary<string, string>()
                            {
                                { "task", "Task API" }
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });



            return services;
        }
    }
}
