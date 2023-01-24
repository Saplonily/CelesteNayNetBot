using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Saladim.SalLogger;

namespace CelesteNyaNetBot;

public class LoggerService
{
    protected StreamWriter? logStream;
    protected readonly IConfiguration configuration;

    public Logger Logger { get; protected set; }

    public LoggerService(IHostApplicationLifetime hostLifetime, IConfiguration configuration)
    {
        this.configuration = configuration;
        Logger = new LoggerBuilder()
            .WithLogToConsole()
            .WithAction(s => logStream?.WriteLine(s))
            .Build();
        hostLifetime.ApplicationStarted.Register(ApplicationStarted);
        hostLifetime.ApplicationStopping.Register(ApplicationStopping);
    }

    protected void ApplicationStarted()
    {
        DateTime now = DateTime.Now;
        string fileName;
        if (configuration["Logging:LogFileName"] is null)
        {
            fileName = Path.Combine("Logs", $"{now.Year}.{now.Month}.{now.Day}.log");
        }
        else
        {
            fileName = configuration["Logging:LogFileName"]!;
        }
        if (!Directory.Exists("Logs"))
            Directory.CreateDirectory("Logs");
        bool doRewrite = false;
        if (File.Exists(fileName))
            doRewrite = true;
        logStream = new StreamWriter(fileName, true, Encoding.UTF8);
        if (doRewrite)
            logStream.Write(new string('\n', 5));
    }

    protected void ApplicationStopping()
    {
        logStream!.Flush();
        logStream.Dispose();
    }

    public void Flush()
    {
        logStream?.Flush();
    }

    public void FlushAndDispose()
    {
        Flush();
        ApplicationStopping();
    }
}
