using Hangfire_background_jobs.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire_background_jobs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherService _Iservices;
        public WeatherForecastController(ILogger<WeatherForecastController> logger,IWeatherService service)
        {
            _logger = logger;
            _Iservices= service;
        }
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task Get()
        {
            for(int i = 0; i < 100; i++)
            {
                var responce =await _Iservices.GetWeatherAsyncs();
                _logger.LogInformation(responce);
            }
        }
    }
}