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
                //检查请求是否为非Json
                if (!context.Request.HasJsonContentType())
                {
                    return Results.Json(new Models.Dtos.Responses.LoginResponse()
                    {
                        success = false,
                        code = ApiCode.UnsupportedContentType,
                        message = "请求有问题",
                        data = null
                    },statusCode:415);
                }

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

                if(!Services.UserService.UserVerification(request.username,request.password,out var userId,out var Role,out _,out _,out var code))
                {
                    string message = "请求处理失败";
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
                        case ApiCode.UnknownError:
                            message = "服务器内部错误";
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
                    Logging.LogReporter.Report(Logging.LogCategory.Unknown,Logging.LogLevel.Error,"用户验证成功,但用户本身数据存在问题 \n - 可能是数据库存在问题，如果您是管理员，建议您手动排查");
                    return Results.Json(new Models.Dtos.Responses.LoginResponse()
                    {
                        success = false,
                        code = ApiCode.InternalProcessingError,
                        message = "服务器内部错误",
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
                Logging.LogReporter.Report(Logging.LogCategory.Json,Logging.LogLevel.Warn,"登录请求的JSON解析失败",ex);

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
                    message = $"服务器内部错误",
                    data = null
                },statusCode:500);
            }
        }
    }
}