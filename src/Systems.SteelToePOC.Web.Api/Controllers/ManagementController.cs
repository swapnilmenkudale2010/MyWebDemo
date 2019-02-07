
namespace Systems.SteelToePOC.Web.Api.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;

    [Route("api/v1.0/[controller]")]
    [ApiController]
    public class ManagementController : APIControllerBase
    {
        public ManagementController()
        {
        }

        /// <summary>
        /// Reports application health information
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("health")]
        public async Task<IActionResult> Health()
        {
            string url = $"{WebApiPath}management/health";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Reports a set of trace information (such as the last 100 HTTP requests)
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("trace")]
        public async Task<IActionResult> Trace()
        {
            string url = $"{WebApiPath}management/trace";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Triggers the application configuration to be reloaded
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("refresh")]
        public async Task<IActionResult> Refresh()
        {
            string url = $"{WebApiPath}management/refresh";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Reports the keys and values from the applications configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("env")]
        public async Task<IActionResult> Env()
        {
            string url = $"{WebApiPath}management/env";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Reports the http request timings for the application
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("metrics/requesttime")]
        public async Task<IActionResult> RequestTime()
        {
            string url = $"{WebApiPath}management/metrics/http.server.request.time";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Reports the http request counts for the application
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("metrics/requestcount")]
        public async Task<IActionResult> RequestCount()
        {
            string url = $"{WebApiPath}management/metrics/http.server.request.count";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Reports CLR threadpool metrics
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("metrics/clrthreadpoolactive")]
        public async Task<IActionResult> ClrThreadpoolActive()
        {
            string url = $"{WebApiPath}management/metrics/clr.threadpool.active";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Reports CLR garbage collections
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("metrics/clrgccollections")]
        public async Task<IActionResult> ClrGcCollections()
        {
            string url = $"{WebApiPath}management/metrics/clr.gc.collections";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Reports CLR threadpool availability
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("metrics/clrthreadpoolavail")]
        public async Task<IActionResult> ClrThreadpoolAvail()
        {
            string url = $"{WebApiPath}management/metrics/clr.threadpool.avail";
            return await SendRequest(url).ConfigureAwait(false);
        }


        /// <summary>
        /// Reports CLR memory used
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("metrics/clrmemoryused")]
        public async Task<IActionResult> ClrMemoryUsed()
        {
            string url = $"{WebApiPath}management/metrics/clr.memory.used";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Generates and reports a snapshot of the applications threads
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("dump")]
        public async Task<IActionResult> Dump()
        {
            string url = $"{WebApiPath}management/dump";
            return await SendRequest(url).ConfigureAwait(false);
        }

        /// <summary>
        /// Generates and downloads a mini-dump of the application
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("heapdump")]
        public async Task<IActionResult> HeapDump()
        {
            string url = $"{WebApiPath}management/heapdump";
            using (var request = CreateRequest(new Uri(url), "application/octet-stream", HttpMethod.Get))
            {
                var response = await HttpClient.SendAsync(request).ConfigureAwait(false);

                var stream = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                var result = new FileContentResult(stream, "application/octet-stream")
                {
                    FileDownloadName = $"heap{DateTime.Now}.gz"
                };
                return result;
            }
        }

        private async Task<IActionResult> SendRequest(string url)
        {
            using (var request = CreateRequest(new Uri(url), "application/json", HttpMethod.Get))
            {
                HttpResponseMessage response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JToken jsonFormatted = JValue.Parse(result);
                    return Ok(jsonFormatted);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}