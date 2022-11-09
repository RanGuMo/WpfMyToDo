using Microsoft.AspNetCore.Mvc;
using MyToDo.API.Context;

namespace MyToDo.API.Controllers
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
        IUnitOfWork<MyToDoContext> unitOfWork;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IUnitOfWork<MyToDoContext> unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var result = unitOfWork.GetRepository<User>();
            var value = result.GetAll();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}