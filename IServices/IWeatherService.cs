namespace Hangfire_background_jobs.IServices
{
    public interface IWeatherService
    {
        public Task<string> GetWeatherAsyncs();

    }
}
