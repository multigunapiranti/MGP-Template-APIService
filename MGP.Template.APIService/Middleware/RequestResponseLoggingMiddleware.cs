using MGP.Template.APIService.Services;
using MGP.Models.NetCoreLibrary31.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Text;
using MGP.Template.APIService.Services;

namespace MGP.Template.APIService.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private ClsAPILogService _clsApiLogService;
        private LogAPI_TH _clsParamLog;
        private StringValues _strAuthorization;
        private StringValues _strClient;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _clsParamLog = new LogAPI_TH();
        }

        public async Task Invoke(HttpContext httpContext, ClsAPILogService apiLogService)
        {
            try
            {
                _clsApiLogService = apiLogService;

                var request = httpContext.Request;
                if (request.Path.StartsWithSegments(new PathString("/api"), StringComparison.OrdinalIgnoreCase))
                {
                    var stopWatch = Stopwatch.StartNew();
                    var requestTime = DateTime.Now;
                    var requestBodyContent = await ReadRequestBody(request);
                    var originalBodyStream = httpContext.Response.Body;
                    var remoteIpAddress = httpContext.Connection.RemoteIpAddress;

                    // Get Header Authorization //
                    if (!httpContext.Request.Headers.TryGetValue("Authorization", out _strAuthorization))
                        _strAuthorization = "No-Auth";

                    // Get Header Client //
                    if (!httpContext.Request.Headers.TryGetValue("x-client", out _strClient))
                        _strClient = "N/A";

                    using (var responseBody = new MemoryStream())
                    {
                        var response = httpContext.Response;
                        response.Body = responseBody;
                        await _next(httpContext);
                        stopWatch.Stop();

                        string responseBodyContent = null;
                        responseBodyContent = await ReadResponseBody(response);
                        await responseBody.CopyToAsync(originalBodyStream);

                        _clsParamLog.authorization = _strAuthorization;
                        _clsParamLog.client = _strClient;
                        _clsParamLog.ip_address = remoteIpAddress.ToString();
                        _clsParamLog.request_time = requestTime;
                        _clsParamLog.response_millis = stopWatch.ElapsedMilliseconds;
                        _clsParamLog.status_code = response.StatusCode;
                        _clsParamLog.method = request.Method;
                        _clsParamLog.api = "MGP.Template.APIService";
                        _clsParamLog.path = request.Path;
                        _clsParamLog.query_string = request.QueryString.ToString();
                        _clsParamLog.request_body = requestBodyContent;
                        _clsParamLog.response_body = responseBodyContent;
                        _clsParamLog.last_modified_time = DateTime.Now;
                        await SafeLog(_clsParamLog);
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (Exception)
            {
                await _next(httpContext);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task SafeLog(LogAPI_TH param)
        {
            if (!string.IsNullOrEmpty(param.client) && param.client.Length > 100)
            {
                param.client = $"(Truncated to 100 chars) {param.client.Substring(0, 100)}";
            }

            if (!string.IsNullOrEmpty(param.api) && param.api.Length > 100)
            {
                param.api = $"(Truncated to 100 chars) {param.api.Substring(0, 100)}";
            }

            if (!string.IsNullOrEmpty(param.path) && param.path.Length > 350)
            {
                param.path = $"(Truncated to 350 chars) {param.path.Substring(0, 350)}";
            }

            if (!string.IsNullOrEmpty(param.authorization) && param.authorization.Length > 500)
            {
                param.authorization = $"(Truncated to 500 chars) {param.authorization.Substring(0, 500)}";
            }

            if (!string.IsNullOrEmpty(param.request_body) && param.request_body.Length > 2500)
            {
                param.request_body = $"(Truncated to 2500 chars) {param.request_body.Substring(0, 2500)}";
            }

            if (!string.IsNullOrEmpty(param.response_body) && param.response_body.Length > 2500)
            {
                param.response_body = $"(Truncated to 2500 chars) {param.response_body.Substring(0, 2500)}";
            }

            if (!string.IsNullOrEmpty(param.query_string) && param.query_string.Length > 500)
            {
                param.query_string = $"(Truncated to 500 chars) {param.query_string.Substring(0, 500)}";
            }

            await _clsApiLogService.Log(new LogAPI_TH
            {
                client = param.client,
                authorization = param.authorization,
                ip_address = param.ip_address,
                request_time = param.request_time,
                response_millis = param.response_millis,
                status_code = param.status_code,
                method = param.method,
                api = param.api,
                path = param.path,
                query_string = param.query_string,
                request_body = param.request_body,
                response_body = param.response_body,
                last_modified_time = param.last_modified_time
            });
        }
    }
}
