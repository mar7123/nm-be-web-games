using Serilog.Core;
using Serilog.Events;

namespace nm_be_web_games.Configurations.Logging;

public class VersionEnricher : ILogEventEnricher
{
    private readonly IConfiguration _configuration;

    public VersionEnricher(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var version = _configuration["AppSettings:Version"];
        if (version != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Version", version));
        }
    }
}
