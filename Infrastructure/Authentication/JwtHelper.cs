using System.IdentityModel.Tokens.Jwt;

internal static class JwtHelper
{
	public static string? GetClaim(string jwtToken, string claimType)
	{
		var handler = new JwtSecurityTokenHandler();
		var token = handler.ReadJwtToken(jwtToken);

		var claim = token.Claims.FirstOrDefault(c => c.Type == claimType);
		return claim?.Value;
	}
}