using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace PlayScattergories.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var result = async (IHubContext<ChatHub> hub, string message) =>
            await hub.Clients.All.SendAsync("NotifyMe", $"Message: {message}");

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("testing")]
        public void Testing()
        {
            //return new List<string>
            //{
            //    "1",
            //    "testing"
            //};
            var result = async (IHubContext<ChatHub> hub, string message) =>
            await hub.Clients.All.SendAsync("NotifyMe", $"Message: {message}");
        }
    }
}
