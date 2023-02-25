using Autofac;
using Autofac.Extensions.DependencyInjection;
using GrpcTelegram;
using Habit.API.GrpcInterceptor;
using Habit.API.Infastructure;
using Habit.API.Infrastructure.Filters;
using Habit.API.Services;
using Hangfire;
using Hangfire.Customization;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace Habit.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<GrpcExceptionInterceptor>();

            services 
                .AddCustomDbContext(Configuration)
                .AddSwagger(Configuration)
                .AddHangfire(Configuration);

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ITelegramService, TelegramService>();
            services.AddTransient<IHabitService, HabitService>();


            services.AddGrpcClient<Telegram.TelegramClient>((services, options) =>
            {
                options.Address = new Uri(Configuration["grpcTelegram"]);
            }).AddInterceptor<GrpcExceptionInterceptor>();


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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IBackgroundJobClient backgroundJobs)
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
                    c.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "HABIT.API V1");
                    c.OAuthClientId("habitswaggerui");
                    c.OAuthAppName("Habit Swagger UI");
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


            //Запускаем джоб на каждый день обнуление кол-во выполненных привычек 
            RecurringJob.AddOrUpdate<IHabitService>(
                "ResetHabits.",
                (x) => x.ResetHabits(),
                "0 0 * * *",
                TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));

            app.UseRouting();
            app.UseCors("CorsPolicy");

            ConfigureAuth(app);
            app.UseEndpoints(endpoins =>
            {
                endpoins.MapDefaultControllerRoute();
                endpoins.MapControllers();
                endpoins.MapHangfireDashboard();
            });
        }

        private void ConfigureAuthService(IServiceCollection services)
        {
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
                options.Audience = "habit";
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
            services.AddDbContext<HabitContext>(options =>
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
                    Title = "Habit HTTP Api",
                    Version = "v1",
                    Description = "The Habit Microservice HTTP Api"
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
                                { "habit", "Habit API" }
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
