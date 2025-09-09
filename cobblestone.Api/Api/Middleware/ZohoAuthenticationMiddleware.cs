using Api.Services;
using System.Security.Claims;

namespace Api.Middleware
{
    public class ZohoAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ZohoAuthenticationMiddleware> _logger;

        public ZohoAuthenticationMiddleware(RequestDelegate next, ILogger<ZohoAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IZohoSessionService zohoSessionService)
        {
            // Check if user is already authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            // Check for Zoho session key in header
            var sessionKey = context.Request.Headers["x-session-key"].FirstOrDefault();
            
            if (!string.IsNullOrEmpty(sessionKey))
            {
                _logger.LogDebug("Found Zoho session key in request");
                
                var principal = await zohoSessionService.ValidateSessionKeyAsync(sessionKey);
                if (principal != null)
                {
                    _logger.LogDebug("Zoho session key validated successfully");
                    context.User = principal;
                }
                else
                {
                    _logger.LogWarning("Invalid Zoho session key: {SessionKey}", sessionKey);
                }
            }

            await _next(context);
        }
    }
}
