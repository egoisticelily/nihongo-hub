namespace NihongoHub.API
{
    public static class AuthAPI
    {
        public static async Task<IResult> Login(HttpContext context)
        {
            var request = await context.Request.ReadFromJsonAsync<Models.Dtos.Requests.LoginRequest>();

            if(request == null)
            {
                return Results.Json(new Models.Dtos.Responses.LoginResponse()
                {
                    success = false,
                    code = 502,
                    message = "请求为空",
                    data = null
                });
            }

            if(!Services.UserService.UserVerification(request.username,request.username,out var userId,out var Role,out var errorMessage,out var errorCategory))
            {
                return Results.Json(new Models.Dtos.Responses.LoginResponse()
                {
                    success = false,
                    code = 502,
                    message = $"[{errorCategory}] - {errorMessage}",
                    data = null
                });
            }
            if(userId == null || Role == null)
            {
                return Results.Json(new Models.Dtos.Responses.LoginResponse()
                {
                   success = false,
                   code = 502,
                   message = "用户数据可能出现问题，如果您是管理员，建议您手动检查 否则，建议您通知管理员",
                   data = null
                });
            }

            var userType = (Models.Entities.UserType)Role;

            Services.TokenService.GenerateToken(new()
            {
                UserId = (int)userId,
                Username = request.username,
                Role = userType
            },out var token,out var expiresAt);          

            return Results.Json(new Models.Dtos.Responses.LoginResponse()
            {
                success = true,
                code = 200,
                message = "OK",
                data = new()
                {
                    token = token,
                    userid = (int)userId,
                    usertype = userType.ToString(),
                    expiresat = expiresAt
                }
            });
        }
    }
}