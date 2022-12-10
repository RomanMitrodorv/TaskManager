using Task.API;
using Task.API.Infastructure;
using TaskManager.WebHost.Customization;


var configuration = GetConfiguration();

Log.Logger = GetLogger(configuration);

try
{
    Log.Information("Configuring web hosting ({ApplicationContext})...", Program.AppName);

    var host = CreateHostBuilder(configuration, args);

    Log.Information("Applying migrations ({ApplicationContext})...", Program.AppName);

    host.MigrateDbContext<TaskContext>((_, __) => { });

    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);

    host.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Program.AppName);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

IWebHost CreateHostBuilder(IConfiguration configuration, string[] args) =>
     Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .CaptureStartupErrors(false)
        .ConfigureKestrel((a, b) => { })
        .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseSerilog()
        .Build();



IConfiguration GetConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddEnvironmentVariables()
        .Build();
}

Serilog.ILogger GetLogger(IConfiguration configuration)
{
    var seqServerUrl = configuration["Serilog:SeqServerUrl"];
    var logstashUrl = configuration["Serilog:LogstashgUrl"];

    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithEnvironmentName()
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ApplicationContext", AppName)
        .WriteTo.Console()
        .WriteTo.Seq(string.IsNullOrEmpty(seqServerUrl) ? "http://seq" : seqServerUrl)
        .WriteTo.Http(string.IsNullOrEmpty(logstashUrl) ? "http://logstash:8080" : logstashUrl)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}