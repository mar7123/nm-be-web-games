using nm_be_web_games.Configurations.Logging;
using Serilog;
using Serilog.Core;

namespace nm_be_web_games.Configuration;

public class SerilogConfigurator
{
    internal const string DefaultConsoleOutputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3} {Version}] {Message:lj}{NewLine}{Exception}";
    public static Logger CreateLogger(IConfiguration configuration)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console(outputTemplate: DefaultConsoleOutputTemplate)
            .Enrich.With(new VersionEnricher(configuration))
            .CreateLogger();
    }
}
