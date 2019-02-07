
namespace Systems.SteelToePOC.Web.Api.Middlewares
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private static ILog _logger;

        public ExceptionHandler(RequestDelegate next)
        {
            this._next = next;
            _logger = LogManager.GetLogger(typeof(ExceptionHandler));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            _logger.Error(exception.Message);
            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
