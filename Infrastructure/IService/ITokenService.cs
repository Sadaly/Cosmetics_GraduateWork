using Microsoft.AspNetCore.Http;

namespace Infrastructure.IService
{
	public interface ITokenService
	{
		string SetJwtToken(HttpResponse response, string token);
		void DeleteJwtToken(HttpResponse response);
		string? GetClaim(string token, string claimType);
	}
}
