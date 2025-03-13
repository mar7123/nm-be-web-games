using System.Text.Json.Serialization;
using nm_be_web_games.Configuration;
using nm_be_web_games.Configurations;
using nm_be_web_games.Repositories;
using nm_be_web_games.Services;
using Serilog;

var config = Configurations.LoadAppConfiguration();

Log.Logger = SerilogConfigurator.CreateLogger(config);

try
{
    Log.Logger.Information("Starting up...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSingleton<IConfiguration>(config);
    builder.Services.AddSingleton<WebSocketRepository>();
    builder.Services.AddSingleton<GameStateRepository>();
    builder.Services.AddSingleton<TaskManager>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.MapControllers();
    app.UseWebSockets();
    app.Run();

}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
