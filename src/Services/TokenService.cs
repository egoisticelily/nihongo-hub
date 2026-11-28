using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace NihongoHub.Services
{
    /// <summary>
    /// 仅作为传递的封装，不加入Models
    /// </summary>
    public class TokenUserData
    {
        public required int UserId {get;set;}
        public required string Username {set;get;}
        public required Models.Entities.UserType Role {get;set;}
    }

    public static class TokenService
    {
        public static void GenerateToken(TokenUserData userData,out string token,out DateTime expiresAt)
        {
            ///后续有计划加入关于用户Ip的部分
        
            var jwtConfig = Init.ConfigInit.Current.Jwt;

            //计算签发时间和过期时间
            DateTime validFrom = DateTime.UtcNow;
            expiresAt = validFrom.AddMinutes(jwtConfig.ExpireMinutes);

            //从 Base64 还原签名密钥
            byte[] keyBytes = Convert.FromBase64String(jwtConfig.Key);

            SymmetricSecurityKey securityKey = new(keyBytes);

            //创建签名凭证

            SigningCredentials signingCredentials = new(securityKey,SecurityAlgorithms.HmacSha256);

            //设置 JWT Claims
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Sub,userData.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName,userData.Username),
                new Claim("role",userData.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            //创建 JWT 对象

            JwtSecurityToken jwt = new(
                issuer: jwtConfig.Issuer, 
                claims: claims, 
                notBefore: validFrom,
                expires: expiresAt,
                signingCredentials: signingCredentials
            );
            token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return;
        }
    }
}