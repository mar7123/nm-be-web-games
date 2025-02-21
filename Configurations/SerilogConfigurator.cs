using Serilog;
using Serilog.Core;

namespace nm_be_web_games.Configuration;

public class SerilogConfigurator
{
    public static Logger CreateLogger()
    {
        var configuration = LoadAppConfiguration();
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            // .Enrich.With(new VersionEnricher(new()))
            .CreateLogger();
    }

    private static IConfigurationRoot LoadAppConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();
    }
}
