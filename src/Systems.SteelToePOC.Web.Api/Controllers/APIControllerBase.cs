
namespace Systems.SteelToePOC.Web.Api.Controllers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class APIControllerBase : ControllerBase
    {
        protected static HttpClient HttpClient { get; set; }

        protected string WebApiPath { set; get; }

        public APIControllerBase()
        {
            WebApiPath = GetConfigValue("AppSettings", nameof(WebApiPath));
            HttpClient = new HttpClient();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="mediaTypeHeaderValue"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        protected static HttpRequestMessage CreateRequest<T>(Uri url, string mediaTypeHeaderValue, HttpMethod method, T content, MediaTypeFormatter formatter)
        {
            var request = new HttpRequestMessage { RequestUri = url };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaTypeHeaderValue));
            request.Method = method;
            request.Content = new ObjectContent<T>(content, formatter);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="mediaTypeHeaderValue"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected static HttpRequestMessage CreateRequest(Uri url, string mediaTypeHeaderValue, HttpMethod method)
        {
            var request = new HttpRequestMessage { RequestUri = url };
            if (!string.IsNullOrWhiteSpace(mediaTypeHeaderValue))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaTypeHeaderValue));
            }
            request.Method = method;
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public static HttpResponseMessage CreateResponse(HttpResponseMessage httpResponse)
        {
            if (httpResponse != null)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                response.StatusCode = httpResponse.StatusCode;
                response.Content = httpResponse.Content;
                return response;
            }
            return httpResponse;
        }

        protected IActionResult CreateResponse(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.BadRequest: return BadRequest();
                case HttpStatusCode.InternalServerError: return StatusCode(StatusCodes.Status500InternalServerError);
                case HttpStatusCode.Unauthorized: return StatusCode(StatusCodes.Status401Unauthorized);
                case HttpStatusCode.NotFound: return StatusCode(StatusCodes.Status404NotFound);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        private static string GetConfigValue(string sectionName, string propertyName)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, false)
                .Build();

            return config?.GetSection(sectionName)?.GetSection(propertyName)?.Value ??
              throw new InvalidOperationException($"{propertyName} could not be found in the config.");
        }
    }
}