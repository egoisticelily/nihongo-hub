using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace NihongoHub.Services
{
    public static class AuthService
    {
        public static bool ValidateToken(string token, out TokenUserData? userData,out string errorMessage,out string errorCode)
        {
            userData = null;
            errorMessage = "";
            errorCode = "";

            if (string.IsNullOrWhiteSpace(token))
            {
                errorMessage = "[ERROR]token为空";
                errorCode = "TOKEN_MISSING";
                return false;
            }

            //获取预先初始化的验证参数
            var validationParameters = Init.JwtInit.Current.ValidationParameters;

            //创建 JWT 处理器
            JwtSecurityTokenHandler handler = new()
            {
                MapInboundClaims = false
            };

            ClaimsPrincipal principal;

            try
            {
                principal = handler.ValidateToken(
                    token,
                    validationParameters,
                    out _
                );
            }
            catch (SecurityTokenExpiredException)
            {
                errorMessage = "[ERROR]token已过期";
                errorCode = "TOKEN_EXPIRED";
                return false;
            }
            catch (SecurityTokenNotYetValidException)
            {
                errorMessage = "[ERROR]token尚未生效";
                errorCode = "TOKEN_NOT_YET_VALID";
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                errorMessage = "[ERROR]token签名无效";
                errorCode = "TOKEN_INVALID_SIGNATURE";
                return false;
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                errorMessage = "[ERROR]token签发者无效";
                errorCode = "TOKEN_INVALID_ISSUER";
                return false;
            }
            catch (SecurityTokenException)
            {
                errorMessage = "[ERROR]token无效";
                errorCode = "TOKEN_INVALID";
                return false;
            }
            catch (ArgumentException)
            {
                errorMessage = "[ERROR]token格式无效";
                errorCode = "TOKEN_INVALID";
                return false;
            }

            string? userIdValue = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            string? username = principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;

            string? roleValue = principal.FindFirst("role")?.Value;


            if (!int.TryParse(userIdValue, out int userId) || userId <= 0)
            {
                errorMessage = "[ERROR]token中的userid无效";
                errorCode = "TOKEN_CLAIMS_INVALID";
                return false;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "[ERROR]token中的username无效";
                errorCode = "TOKEN_CLAIMS_INVALID";
                return false;
            }

            if(!Enum.TryParse(roleValue,true,out Models.Entities.UserType role) || !Enum.IsDefined(typeof(Models.Entities.UserType),role))
            {
                errorMessage = "[ERROR]token中的role无效";
                errorCode = "TOKEN_CLAIMS_INVALID";
                return false;
            }

            userData = new()
            {
                UserId = userId,
                Username = username,
                Role = role
            };

            return true;
        }
    }
}