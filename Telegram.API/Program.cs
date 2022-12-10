using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Net;
using Telegram.API;
using Telegram.API.Service;

var configuration = GetConfiguration();

Log.Logger = GetLogger(configuration);

try
{
    Log.Information("Configuring web hosting ({ApplicationContext})...", Program.AppName);

    var host = CreateHostBuilder(configuration, args);


    Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);

    var telegram = host.Services.GetRequiredService<ITelegramService>();


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
    WebHost.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .CaptureStartupErrors(false)
        .ConfigureKestrel(options =>
        {
            var ports = GetDefinedPorts(configuration);
            options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
            });

            options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
            });

        })
        .UseStartup<Startup>()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseSerilog()
        .Build();


(int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
{
    var grpcPort = config.GetValue("GRPC_PORT", 5001);
    var port = config.GetValue("PORT", 80);
    return (port, grpcPort);
}


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
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}

public partial class Program
{
    public static string Namespace = typeof(Startup).Namespace;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
}