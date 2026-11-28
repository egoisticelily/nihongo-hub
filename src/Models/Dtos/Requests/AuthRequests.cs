///登录、注册等身份验证相关请求

namespace NihongoHub.Models.Dtos.Requests
{
    public class LoginRequest
    {
        public required string username {get;set;}
        public required string password {get;set;}
    }
}