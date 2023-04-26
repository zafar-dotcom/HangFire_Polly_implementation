using Hangfire_background_jobs.IServices;
using Hangfire_background_jobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire_background_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RateLimitApiController : ControllerBase
    {
        private readonly IDAL _dal;
        public RateLimitApiController(IDAL dal)
        {
            _dal = dal;
        }
        [HttpGet]
        [Route("getallemployee")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<TransactionModel> GetListOfApplicatns()
        {
            var list = _dal.GetfromMasterDEtail();
            return list;
        }
        [HttpPut]
        [Route("update_app_exp")]
        public IActionResult Update_Applicant_Experience(TransactionModel mdl)
        {
           bool result= _dal.Update_master_detail(mdl);
            if (result == true)
            {
                return Ok();

            }
            else
            {
                return BadRequest("update failed");
            }
        }

    }
}
