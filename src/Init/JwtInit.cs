using Microsoft.IdentityModel.Tokens;

namespace NihongoHub.Init
{
    public class JwtValidationConfig
    {
        public JwtValidationConfig()
        {
            var jwtConfig = ConfigInit.Current.Jwt;

            ValidationParameters = new()
            {
                //验证签名密钥
                ValidateIssuerSigningKey = true,
                //Base64 还原的密钥
                IssuerSigningKey = new SymmetricSecurityKey(
                    Convert.FromBase64String(jwtConfig.Key)
                ),
                //验证签发者
                ValidateIssuer = true,
                //允许的签发者
                ValidIssuer = jwtConfig.Issuer,
                //验证受众
                ValidateAudience = false,
                //验证 Token 有效时间
                ValidateLifetime = true,
                //Token 必须存在过期时间
                RequireExpirationTime = true,
                //时钟偏差
                ClockSkew = TimeSpan.FromSeconds(30),
                //指定的签名算法
                ValidAlgorithms = new[]
                {
                    SecurityAlgorithms.HmacSha256
                }
            };
        }

        public TokenValidationParameters ValidationParameters
        {
            get;
        }
    }
    public static class JwtInit
    {
        private static JwtValidationConfig? _jwtConfig;
        public static JwtValidationConfig Current
        {
            get
            {
                if(_jwtConfig == null)
                {
                    Logging.LogReporter.Fatal("[内部错误]Jwt验证配置未能初始化",Logging.LogCategory.Unknown);
                }
                return _jwtConfig;
            }
        }

        public static void Init()
        {
            _jwtConfig = new();
        }
    }
}