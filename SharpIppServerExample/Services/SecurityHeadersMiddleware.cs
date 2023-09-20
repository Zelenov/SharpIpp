using Microsoft.Extensions.Options;

namespace SharpIppServerExample.Services
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware( RequestDelegate next )
        {
            _next = next;
        }

        public async Task InvokeAsync( HttpContext httpContext )
        {
            httpContext.Response.Headers.TryAdd( "X-Frame-Options", "DENY" );
            httpContext.Response.Headers.TryAdd( "Content-Security-Policy", $"default-src 'none';" );
            httpContext.Response.Headers.TryAdd( "X-Content-Type-Options", "nosniff" );
            httpContext.Response.Headers.TryAdd( "Referrer-Policy", "strict-origin-when-cross-origin" );
            httpContext.Response.Headers.TryAdd( "Permissions-Policy", "camera=(), geolocation=(), microphone=()" );
            httpContext.Response.Headers.TryAdd( "Cross-Origin-Opener-Policy", "unsafe-none" );
            httpContext.Response.Headers.TryAdd( "Cross-Origin-Embedder-Policy", "unsafe-none" );
            httpContext.Response.Headers.TryAdd( "Cross-Origin-Resource-Policy", "same-origin" );
            await _next( httpContext );
        }
    }
}
