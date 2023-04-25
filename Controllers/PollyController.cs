using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire_background_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollyController : ControllerBase
    {
        private readonly IHttpClientFactory _clientfactroy;
        public PollyController(IHttpClientFactory clientfactroy)
        {
            _clientfactroy = clientfactroy;
        }
        [HttpGet]
        [Route("get")]
        public async Task<string> GetAsyncs()
        {
            //define 404 page
          //  var url500 = $"https://localhost:7054";
            var url = $"https://www.c-sharpcorner.com/mytestpagefor404";
            var cleint = _clientfactroy.CreateClient("csharpcorner");
            var responce = await cleint.GetAsync(url);
         // var responce = await cleint.GetAsync(url500);
            var result = await responce.Content.ReadAsStringAsync();
            return result;

        }
    }
}
