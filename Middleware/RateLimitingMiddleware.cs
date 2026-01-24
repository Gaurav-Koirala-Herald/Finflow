using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;

namespace FinFlowAPI.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly ConcurrentDictionary<string, RateLimitInfo> _requests
            = new ConcurrentDictionary<string, RateLimitInfo>();

        private const int LIMIT = 50;         
        private static readonly TimeSpan WINDOW = TimeSpan.FromMinutes(1);

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(ip))
            {
                await _next(context);
                return;
            }

            var now = DateTime.UtcNow;

            var rateLimitInfo = _requests.GetOrAdd(ip, _ =>
                new RateLimitInfo
                {
                    FirstRequestTime = now,
                    RequestCount = 0
                });

            lock (rateLimitInfo)
            {
                if (now - rateLimitInfo.FirstRequestTime > WINDOW)
                {
                    // Reset window
                    rateLimitInfo.FirstRequestTime = now;
                    rateLimitInfo.RequestCount = 1;
                }
                else
                {
                    rateLimitInfo.RequestCount++;
                }

                if (rateLimitInfo.RequestCount > LIMIT)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        message = "Too many requests. Please try again later.",
                        retryAfterSeconds = (int)(WINDOW - (now - rateLimitInfo.FirstRequestTime)).TotalSeconds
                    };

                    context.Response.Headers["Retry-After"] =
                        response.retryAfterSeconds.ToString();

                    context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
            }

            await _next(context);
        }

        private class RateLimitInfo
        {
            public DateTime FirstRequestTime { get; set; }
            public int RequestCount { get; set; }
        }
    }
}
