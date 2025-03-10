using Serilog.Core;
using Serilog.Events;

namespace nm_be_web_games.Configurations.Logging;

class ThreadEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ThreadId", Environment.CurrentManagedThreadId));
    }
}
