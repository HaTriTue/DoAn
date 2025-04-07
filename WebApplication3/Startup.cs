using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Security.Claims;

[assembly: OwinStartup(typeof( WebApplication3.Startup))]
namespace WebApplication3
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 1. Cấu hình Cookie Authentication (phải đặt trước Google Authentication)
            app.SetDefaultSignInAsAuthenticationType("ExternalCookie");

            System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ExternalCookie",
                LoginPath = new PathString("/Users/Login"), // Đường dẫn khi chưa đăng nhập
                CookieSecure = CookieSecureOption.Always // Chỉ dùng cookie qua HTTPS (có thể bỏ nếu chạy local)
            }); app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "397393632069-kf19m1a4e1beq9nbao6q1nv8u9h77uod.apps.googleusercontent.com",
                ClientSecret = "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",
                CallbackPath = new PathString("/signin-google"), // Thêm đường dẫn callback

                SignInAsAuthenticationType = "ExternalCookie" // Phải khớp với AuthenticationType ở trên

            });
        }
    }
}