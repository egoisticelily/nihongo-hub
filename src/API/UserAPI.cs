using NihongoHub.Models.Code;

namespace NihongoHub.API
{
    public static class UserAPI
    {
        /// <summary>
        /// 注:尚未实现密码更改后的token处理问题
        /// 实际上密码更改后，原token仍然被接受
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IResult> UserPasswordChange(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.PasswordChangeRequest>(context);

            if (!requestResult.Success)
            {
                string message = "";
                switch (requestResult.Code)
                {
                    case ApiCode.UnsupportedContentType:
                        message = "请求有问题";
                        break;
                    case ApiCode.EmptyRequest:
                        message = "请求为空";
                        break;
                    case ApiCode.InvalidJson:
                        message = "请求错误";
                        break;
                    case ApiCode.UnknownError:
                        message = "服务器内部错误";
                        break;
                    default:
                        message = "请求处理失败";
                        break;
                }

                return Results.Json(new Models.Dtos.Responses.ApiResponse()
                {
                    success = false,
                    code = requestResult.Code,
                    message = message,
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(requestResult.Code));
            }

            var request = requestResult.Request;

            if(!Utilities.UserValidator.ValidateUser(request.token,out var userData,out var code))
            {
                string message = "";
                switch (code)
                {
                    case ApiCode.TokenMissing:
                        message = "token错误";
                        break;
                    case ApiCode.UnknownError:
                        message = "未知错误";
                        break;
                    default:
                        message = "查询时出现错误";
                        break;
                }

                return Results.Json(new Models.Dtos.Responses.ApiResponse()
                {
                    success = false,
                    code = code, 
                    message = message,
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            if(request.data == null)
            {
                return Results.Json(new Models.Dtos.Responses.ApiResponse()
                {
                    success = false,
                    code = ApiCode.EmptyRequest, 
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }

            if(!Services.UserService.UserPasswordChange(userData.UserId,request.data.password,out var errorMessage,out var errorCategory))
            {
                switch (errorCategory)
                {
                    case Logging.LogCategory.Database:
                        code = ApiCode.DatabaseError;
                        break;
                    case Logging.LogCategory.Unknown:
                        code = ApiCode.UnknownError;
                        break;
                    case Logging.LogCategory.None:
                        code = ApiCode.InvalidParameter;
                        break;
                    default:
                        code = ApiCode.UnknownError;
                        break;
                }

                return Results.Json(new Models.Dtos.Responses.ApiResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{errorCategory}] {errorMessage}",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.ApiResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }
    }
}