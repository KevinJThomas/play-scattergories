using Microsoft.Extensions.Logging.AzureAppServices;
using PlayScattergories.Server.Helpers;
using PlayScattergories.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddAzureWebAppDiagnostics();
builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "logs-";
    options.FileSizeLimit = 50 * 1024;
    options.RetainedFileCountLimit = 5;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("/chatHub");
//app.MapGet("/test", async (IHubContext<ChatHub> hub, string message) =>
//  await hub.Clients.All.SendAsync("NotifyMe", $"Message: {message}"));

app.MapFallbackToFile("/index.html");

app.UseCors(options =>
{
    options.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .SetIsOriginAllowed(origin => true);
});

ConfigurationHelper.Initialize(app.Configuration);
var logFactory = new LoggerFactory();
var logger = logFactory.CreateLogger<Program>();
logger.LogInformation("this is debug log");
LobbyService.Initialize(logger);

app.Run();
