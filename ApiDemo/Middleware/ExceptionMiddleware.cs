using Application.Exceptions;

namespace ApiDemo.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext ctx)
        {
            try
            {
                await _next(ctx);
            }
            catch (NotFoundException ex)
            {
                await WriteError(ctx, 404, ex.Message);
            }
            catch (UnauthorizedException ex)
            {
                await WriteError(ctx, 401, ex.Message);
            }
            catch (ConflictException ex)
            {
                await WriteError(ctx, 409, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteError(ctx, 500, "Internal server error");
            }
        }

        private static async Task WriteError(HttpContext ctx, int code, string message)
        {
            ctx.Response.StatusCode = code;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(new { error = message, statusCode = code });
        }
    }
}
