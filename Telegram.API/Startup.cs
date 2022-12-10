using Autofac;
using Autofac.Extensions.DependencyInjection;
using GrpcTelegram;
using MongoDB.Driver;
using Serilog;
using Telegram.API.Repository;
using Telegram.API.Service;

namespace Telegram.API
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
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            });


            services
                .AddMongoDb(Configuration);

            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);

            return new AutofacServiceProvider(containerBuilder.Build());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var pathBase = Configuration["PATH_BASE"];

            app.UseRouting();

            app.UseStaticFiles();

            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("USING PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<TelegramGrpcService>();
                endpoints.MapGet("/_proto/", async ctx =>
                {
                    ctx.Response.ContentType = "text/plain";
                    using var fs = new FileStream(Path.Combine(env.ContentRootPath, "Proto", "telegram.proto"), FileMode.Open, FileAccess.Read);
                    using var sr = new StreamReader(fs);
                    while (!sr.EndOfStream)
                    {
                        var line = await sr.ReadLineAsync();
                        if (line != "/* >>" || line != "<< */")
                        {
                            await ctx.Response.WriteAsync(line);
                        }
                    }
                });
            });
        }
    }

    public static class StartupExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration Configuration)
        {
            Log.Logger.Information(Configuration["MongoConnectionString"]);
            var mongoClient = new MongoClient(Configuration["MongoConnectionString"]);

            services.AddSingleton<IMongoClient>(mongoClient);

            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton<IMessageRepository, MessageRepository>();

            services.AddSingleton<ITelegramService, TelegramService>();

            return services;
        }


    }
}
