
namespace Systems.SteelToePOC.Web.Api.Controllers
{
    using log4net;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class NudgeController : ControllerBase
    {
        private readonly ILog _logger;
        public NudgeController(ILog log)
        {
            _logger = log;
        }

        [HttpGet, Route("")]
        public ActionResult<bool> Get()
        {
            _logger.Info("NudgeController - Get");
            return true;
        }
    }
}
