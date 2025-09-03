using Application.Abstractions;
using Domain.Entity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication
{
	internal sealed class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
	{
		private readonly JwtOptions _options = options.Value;

		public string Generate(User user)
		{
			var claims = new Claim[] {
				new (ClaimTypes.NameIdentifier, user.Id.ToString()),
				new (ClaimTypes.Email, user.Email.Value),
				new (ClaimTypes.Role, user.GetType().Name)
			};

			var signingCredentials = new SigningCredentials(
				new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(_options.SecretKey)),
				SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				_options.Issuer,
				_options.Audience,
				claims,
				null,
				DateTime.UtcNow.AddYears(1),
				signingCredentials
				);

			string tokenValue = new JwtSecurityTokenHandler()
				.WriteToken(token);

			return tokenValue;
		}
	}
}
