using System.Text.Json;
using NihongoHub.Models.Code;

namespace NihongoHub.API
{
    public static class VocabularyNotebookAPI
    {
        public static async Task<IResult> SearchAllVocabularyNotebook(HttpContext context)
        {
            try
            {
                //检查请求是否为非Json
                if (!context.Request.HasJsonContentType())
                {
                    return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                    {
                        success = false,
                        code = ApiCode.UnsupportedContentType,
                        message = "请求有问题",
                        data = null
                    },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.UnsupportedContentType));
                }

                var request = await context.Request.ReadFromJsonAsync<Models.Dtos.Requests.SearchVocabularyNotebookByUseridRequest>();

                if(request == null)
                {
                    return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                    {
                        success = false,
                        code = ApiCode.EmptyRequest,
                        message = "请求为空",
                        data = null
                    },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
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
                        code = ApiCode.EmptyRequest, 
                        message = "用户状态有问题",
                        data = null
                    },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
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
            catch(JsonException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Json,Logging.LogLevel.Warn,"登录请求的JSON解析失败",ex);

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
                {
                    success = false,
                    code = ApiCode.InvalidJson,
                    message = $"请求错误",
                    data = null
                },statusCode:400);
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Unknown,Logging.LogLevel.Error,"出现未处理异常",ex);
                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookSummaryByUseridResponse()
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