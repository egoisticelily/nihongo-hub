using System.Text.Json;
using NihongoHub.Models.Code;

namespace NihongoHub.API
{
    public static class VocabularyNotebookAPI
    {
        public static async Task<IResult> SearchAllVocabularyNotebook(HttpContext context)
        {
            var _request = await ValidateRequest<Models.Dtos.Requests.SearchVocabularyNotebookByUseridRequest>(context);

            if (!_request.success)
            {
                string message = "";
                switch (_request.code)
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

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                {
                    success = false,
                    code = _request.code,
                    message = message,
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(_request.code));
            }

            var request = _request.request;

            if(request == null)
            {
                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                {
                    success = false,
                    code = ApiCode.UnknownError,
                    message = "出现未知问题",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.UnknownError));
            }

            if(String.IsNullOrWhiteSpace(request.token))
            {
                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                {
                    success = false,
                    code = ApiCode.TokenMissing, 
                    message = "token错误",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.TokenMissing));
            }

            if(!Services.AuthService.ValidateToken(request.token,out var userData,out _,out var code))
            {
                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                {
                    success = false,
                    code = code, 
                    message = "token错误",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            if(userData == null)
            {
                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                {
                    success = false,
                    code = ApiCode.UnknownError, 
                    message = "用户状态有问题",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.UnknownError));
            }

            if(!Services.UserService.ValidateUserIdentity(userData,out _,out _,out code))
            {
                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                {
                    success = false,
                    code = code, 
                    message = "用户状态有问题",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            var notebookList = Services.VocabularyNotebookService.FindVocabularyNotebooksByUserid(userData.UserId,out var failures);

            if(failures != null)
            {
                switch (failures.errorCategory)
                {
                    case Logging.LogCategory.Database:
                        code = ApiCode.DatabaseError;
                        break;
                    case Logging.LogCategory.Unknown:
                        code = ApiCode.UnknownError;
                        break;
                    default:
                        code = ApiCode.UnknownError;
                        break;
                }

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{failures.errorCategory}] {failures.errorMessage}",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
                data = new()
                {
                    VocabularyNotebookList = Models.Dtos.Responses.Auxiliary.DatabaseVocabularyNotebookToResponseData(notebookList)
                }
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }

        private class Request<T>
        {
            public Request(int _code,T? _request)
            {
                code = _code;
                request = _request;
            }
            public bool success
            {
                get
                {
                    return code == ApiCode.Success;
                }
            }
            public int code {get;}
            public T? request {get;}
        }
        private async static Task<Request<T>> ValidateRequest<T>(HttpContext context) 
            where T : class
        {
            try
            {
                //检查请求是否为非Json
                if (!context.Request.HasJsonContentType())
                {
                    int code = ApiCode.UnsupportedContentType;
                    return new(code,default);
                }

                var request = await context.Request.ReadFromJsonAsync<T>();

                if(request == null)
                {
                    int code = ApiCode.EmptyRequest;
                    return new(code,request);
                }
                return new(ApiCode.Success,request);
            }
            catch(JsonException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Json,Logging.LogLevel.Warn,"JSON解析失败",ex);
                int code = ApiCode.InvalidJson;
                return new(code,default);
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Unknown,Logging.LogLevel.Error,"出现未处理异常",ex);
                int code = ApiCode.UnknownError;
                return new(code,default);
            }
        }
    }
}