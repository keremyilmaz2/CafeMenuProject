namespace Calia.Services.ProductAPI.Middleware
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Serilog;

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // İstek Bilgilerini Logla
            Log.Information("📥 [REQUEST] {Method} {Url}", context.Request.Method, context.Request.Path);

            await _next(context);  // Sonraki middleware çalıştır

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Cevap Bilgilerini Logla
            Log.Information("📤 [RESPONSE] {StatusCode} {Url} - {ElapsedMs} ms",
                context.Response.StatusCode, context.Request.Path, elapsedMs);
        }
    }

}
