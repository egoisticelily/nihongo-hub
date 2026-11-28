///身份验证相关响应

namespace NihongoHub.Models.Dtos.Responses
{
    public class LoginResponseData
    {
        public required string token {get;set;}
        public required int userid {get;set;}
        public required string usertype {get;set;}
        public required DateTime expiresat {get;set;}
    }
    public class LoginResponse : ApiResponse<LoginResponseData>;
}