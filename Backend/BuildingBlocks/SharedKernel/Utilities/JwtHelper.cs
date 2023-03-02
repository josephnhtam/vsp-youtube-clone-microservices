using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SharedKernel.Utilities {
    public static class JwtHelper {

        public static string GenerateJWT (string secretKey, string? algorithm, Action<SecurityTokenDescriptor> configureDescriptor) {
            return GenerateJWT(Encoding.UTF8.GetBytes(secretKey), algorithm, configureDescriptor);
        }

        public static string GenerateJWT (byte[] secretKey, string? algorithm, Action<SecurityTokenDescriptor> configureDescriptor) {
            if (algorithm == null) {
                algorithm = SecurityAlgorithms.HmacSha256Signature;
            }

            var signingKey = new SymmetricSecurityKey(secretKey);

            var descriptor = new SecurityTokenDescriptor();
            descriptor.SigningCredentials = new SigningCredentials(signingKey, algorithm);

            configureDescriptor.Invoke(descriptor);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

        public static bool ValidateJWT (string jwt, string secretKey, out ClaimsPrincipal claimsPrincipal, Action<TokenValidationParameters> configureParameter) {
            return ValidateJWT(jwt, Encoding.UTF8.GetBytes(secretKey), out claimsPrincipal, configureParameter);
        }

        public static bool ValidateJWT (string jwt, byte[]? secretKey, out ClaimsPrincipal claimsPrincipal, Action<TokenValidationParameters> configureParameter) {
            try {
                var tokenHandler = new JwtSecurityTokenHandler();

                var parameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = secretKey != null,
                    IssuerSigningKey = secretKey != null ? new SymmetricSecurityKey(secretKey) : null
                };

                configureParameter.Invoke(parameters);
                claimsPrincipal = tokenHandler.ValidateToken(jwt, parameters, out var _);

                return true;
            } catch (Exception) {
                claimsPrincipal = null!;
                return false;
            }
        }

    }
}
