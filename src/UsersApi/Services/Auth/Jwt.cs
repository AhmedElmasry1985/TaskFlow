using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UsersApi.Models;

namespace UsersApi.Services.Auth;

public class Jwt
{
    private static bool _isInitialized;
    private static string SecretKey, Issuer, Audience;
    public static double? ExpiryMinutes { private set; get; }

    public Jwt(IConfiguration configuration)
    {
        if (_isInitialized) return;
        var jwtSettings = configuration.GetSection("JwtSettings");
        SecretKey = jwtSettings.GetSection("SecretKey").Value;
        Issuer = jwtSettings.GetSection("Issuer").Value;
        Audience = jwtSettings.GetSection("Audience").Value;
        ExpiryMinutes ??= double.Parse(jwtSettings.GetSection("ExpiryMinutes").Value);
        _isInitialized = true;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new (ClaimTypes.Name, user.Username),
            new (ClaimTypes.Role, user.Role),
            new (ClaimTypes.Expiration, ExpiryMinutes.Value.ToString()),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(ExpiryMinutes.Value),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = creds,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}