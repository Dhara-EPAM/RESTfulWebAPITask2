using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RESTfulWebAPITask2.Middleware
{
    public class TokenLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null)
                {
                    var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                    Console.WriteLine($"Token Log: Role={role}, Token={token}");
                }
            }

            await _next(context);
        }
    }
}
