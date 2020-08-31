using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using System.Threading.Tasks;

namespace WebApplication2
{
    public class LoginWithActiveDirectoryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _loginPath = "/Login/index";

        public LoginWithActiveDirectoryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            bool isAthenticatedWithAD = IsAuthenticatedWithActiveDirectory(httpContext);

            bool isAllowedAnonymous = IsAllowedAnonymous(httpContext);
            
            if (!isAllowedAnonymous && !isAthenticatedWithAD)
            {
                httpContext.Response.Redirect(_loginPath);
            }

            return _next(httpContext);
        }

        private static bool IsAuthenticatedWithActiveDirectory(HttpContext httpContext)
        {
            return (httpContext.User.Identity is WindowsIdentity winIdentity &&
                                                winIdentity.IsAuthenticated);
        }

        private static bool IsAllowedAnonymous(HttpContext httpContext)
        {
            var endpoint = httpContext.GetEndpoint();
            var anonymousMethods = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>();
          
            return anonymousMethods is object;
        }

    }
    public static class LoginWithActiveDirectoryMiddlewareExtensions
    {
        public static IApplicationBuilder UseActiveDirMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginWithActiveDirectoryMiddleware>();
        }
    }
}
