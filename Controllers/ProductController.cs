using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire_background_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("login")]
        public string Login()
        {
            // fire and forgot jobs ,execute only once at certain condition.
            var jobid = BackgroundJob.Enqueue(() => Console.WriteLine("welcome to shoping world"));
            return $"job id : {jobid}. welcome mail sent to the customer ";
        }

        [HttpGet]
        [Route("checkout")]
        public string Checkout()
        {
            //delayed jobs : this job runs onece but not imdieately but after some time we have set
            var jobid = BackgroundJob.Schedule(() => Console.WriteLine("you have checked new product"),TimeSpan.FromSeconds(20));
            return $"job id : {jobid}. you have added new prodcut";
        }
        [HttpGet]
        [Route("payment")]
        public string Payment()
        {
            // fire and forgot (parent job) :execute only once 
            var parentjob = BackgroundJob.Enqueue(() => Console.WriteLine("you have done with payment successfully"));
            //continuation job (child job and will executed after parent job executed successfully) : 
            BackgroundJob.ContinueJobWith(parentjob, () => Console.WriteLine("product receipt sent successfully"));
            return "you have done with payment successfully and recipt sent to you";
        }
        [HttpGet]
        [Route("dailyoffer")]
        public string DailyOffer()
        {
            // Recuring job :this job runs many time on the specified cron time
            RecurringJob.AddOrUpdate(() => Console.WriteLine("sent similar product ofer and suggestion"), Cron.Daily);
            return "offer sent";
        }
    }
}
