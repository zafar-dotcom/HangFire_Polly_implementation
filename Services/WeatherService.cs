using Hangfire_background_jobs.IServices;

namespace Hangfire_background_jobs.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly ILogger<WeatherService> _logger;
        private readonly HttpClient _client;
        public WeatherService(ILogger<WeatherService> logger, HttpClient httpclient)
        {
                _logger= logger;
            _client= httpclient;
        }
        public async Task<string> GetWeatherAsyncs()
        {
           


            var url = $"{_client.BaseAddress}WeatherForecast";
            var response =await _client.GetAsync(url).Result.Content.ReadAsStringAsync();
            _logger.LogInformation(response);
            return response;

        }
    }
}
