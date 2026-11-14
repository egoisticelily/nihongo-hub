using System.Text.Json;
using NihongoHub.Models.Code;

namespace NihongoHub.API
{
    public static class AuthAPI
    {
        public static async Task<IResult> Login(HttpContext context)
        {
            try
            {
                var request = await context.Request.ReadFromJsonAsync<Models.Dtos.Requests.LoginRequest>();

                if(request == null)
                {
                    return Results.Json(new Models.Dtos.Responses.LoginResponse()
                    {
                        success = false,
                        code = ApiCode.EmptyRequest,
                        message = "请求为空",
                        data = null
                    },statusCode:400);
                }

                if(!Services.UserService.UserVerification(request.username,request.password,out var userId,out var Role,out var errorMessage,out var errorCategory,out var code))
                {
                    string message = "";
                    switch (code)
                    {
                        case ApiCode.InvalidParameter:
                            message = "用户名或密码参数不符合要求";
                            break;
                        case ApiCode.DatabaseError:
                            message = "服务器内部错误";
                            break;
                        case ApiCode.InvalidCredentials:
                            message = "用户名或密码错误";
                            break;
                        case ApiCode.Success:
                            message = "OK";
                            break;
                    }
                    return Results.Json(new Models.Dtos.Responses.LoginResponse()
                    {
                        success = false,
                        code = code,
                        message = message,
                        data = null
                    },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
                }
                if(userId == null || Role == null)
                {
                    return Results.Json(new Models.Dtos.Responses.LoginResponse()
                    {
                    success = false,
                    code = ApiCode.InternalProcessingError,
                    message = "用户数据可能出现问题，如果您是管理员，建议您手动检查 否则，建议您通知管理员",
                    data = null
                    },statusCode:500);
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
                    code = ApiCode.Success,
                    message = "OK",
                    data = new()
                    {
                        token = token,
                        userid = (int)userId,
                        usertype = userType.ToString(),
                        expiresat = expiresAt
                    }
                },statusCode:200);
            }
            catch(JsonException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Unknown,Logging.LogLevel.Error,"登录过程中出现未处理异常",ex);

                return Results.Json(new Models.Dtos.Responses.LoginResponse()
                {
                    success = false,
                    code = ApiCode.InvalidJson,
                    message = $"请求错误",
                    data = null
                },statusCode:400);
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Unknown,Logging.LogLevel.Error,"登录过程中出现未处理异常",ex);
                return Results.Json(new Models.Dtos.Responses.LoginResponse()
                {
                    success = false,
                    code = ApiCode.UnknownError,
                    message = $"未知错误",
                    data = null
                },statusCode:500);
            }
        }
    }
}