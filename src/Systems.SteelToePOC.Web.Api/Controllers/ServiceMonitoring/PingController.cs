
namespace Systems.SteelToePOC.Web.Api.Controllers
{
    using log4net;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly ILog _logger;
        public PingController(ILog log)
        {
            _logger = log;
        }

        [HttpGet, Route("")]
        public ActionResult<bool>  Get()
        {
            _logger.Info("PingController - Get");
            return true;
        }
    }
}
