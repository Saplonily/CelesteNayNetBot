using CelesteNyaNetBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SaladimQBot.Extensions;

namespace CelesteNyaNetBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .ConfigureAppConfiguration(ConfigureAppConfiguration)
            .Build();

        _ = host.StartAsync();

        LoggerService s = host.Services.GetRequiredService<LoggerService>();
        s.Logger.LogInfo(nameof(Program), "Starting CelesteNyaNetBot service...");

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception e)
            {
                s.Logger.LogFatal(nameof(Program), e, "Fatal exception:");
                s.FlushAndDispose();
                try
                {
                    host.StopAsync();
                }
                catch (Exception) { }
            }
        };
        AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
        {
            s.Logger.LogInfo(nameof(Program), "Program is exiting...");
            host.StopAsync();
        };

        var bot = host.Services.GetRequiredService<CelesteNyaNetBot>();

        await bot.StartAsync();

        while (true)
        {
            string cmd;
            cmd = Console.ReadLine()!;
            if (cmd is "exit")
                break;
        }

        await bot.StopAsync();
        Console.WriteLine("Bot service stopped. Press any key to continue.");
        Console.ReadKey();

    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<CelesteNyaNetBot>();
#if DEBUG
        services.AddSingleton<INyaService, NyaService>();
#else
        services.AddSingleton<INyaService, NyaService>();
#endif
        services.AddSingleton<LoggerService>();
        services.AddSingleton<MemorySessionService>();

        services.AddSingleton<CoroutineService>();
        services.AddSimCommand(
            s => new(t => (CommandModule)s.GetRequiredService(t), s.GetRequiredService<IConfiguration>()["Bot:CommandPrefixes"]!.Split(' ')),
                typeof(Program).Assembly
            );
        services.AddSingleton<DrawingService>();
    }

    public static void ConfigureAppConfiguration(IConfigurationBuilder configuration)
    {
        configuration.AddJsonFile("BotSettings.json");
        configuration.AddJsonFile("KeySettings.json");
#if DEBUG
        configuration.AddJsonFile("DebugSettings.json");
#endif
    }
}