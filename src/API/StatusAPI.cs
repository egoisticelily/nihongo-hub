using NihongoHub.Models.Code;

namespace NihongoHub.API
{
    public static class StatusAPI
    {
        public static async Task<IResult> HeartbeatDetection(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.ApiRequest>(context);

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
                string message = "查询时出现错误";

                return Results.Json(new Models.Dtos.Responses.ApiResponse()
                {
                    success = false,
                    code = code, 
                    message = message,
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            code = ApiCode.Success;
            return Results.Json(new Models.Dtos.Responses.ApiResponse()
            {
                success = true,
                code = code, 
                message = "OK",
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
        }
    }
}