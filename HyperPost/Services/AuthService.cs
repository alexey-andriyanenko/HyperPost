using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HyperPost.Services
{
    public static class AuthService
    {
        public const string ISSUER = "HyperPostServer";
        public const string AUDIENCE = "HyperPostClient";
        public const int LIFETIME = 1;

        private const string KEY = "hyper-post-security-key";

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }

        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = ISSUER,
                ValidateAudience = true,
                ValidAudience = AUDIENCE,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSymmetricSecurityKey(),
            };
        }
    }
}
