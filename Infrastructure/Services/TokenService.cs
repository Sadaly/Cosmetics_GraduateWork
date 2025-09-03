using Infrastructure.IService;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
	public class TokenService : ITokenService
	{
		public void DeleteJwtToken(HttpResponse response)
		{
			response.Cookies.Delete("access_token");
		}

		public string? GetClaim(string token, string claimType)
		{
			return JwtHelper.GetClaim(token, claimType);
		}

		public string SetJwtToken(HttpResponse response, string token)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTimeOffset.UtcNow.AddYears(1)
			};

			response.Cookies.Delete("access_token");
			response.Cookies.Append("access_token", token, cookieOptions);

			//string? userId = JwtHelper.GetClaim(token, "sub");

			return token;
		}
	}
}
