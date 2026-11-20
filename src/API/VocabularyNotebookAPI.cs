using NihongoHub.Models.Code;

namespace NihongoHub.API
{
    public static class VocabularyNotebookAPI
    {
        public static async Task<IResult> SearchAllVocabularyNotebook(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.SearchVocabularyNotebookByUseridRequest>(context);

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

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
                {
                    success = false,
                    code = requestResult.Code,
                    message = message,
                    data = null
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

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
                {
                    success = false,
                    code = code, 
                    message = message,
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

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{failures.errorCategory}] {failures.errorMessage}",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
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

        public static async Task<IResult> SearchVocabularyNotebookByTag(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.SearchVocabularyNotebookByTagRequest>(context);

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

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
                {
                    success = false,
                    code = requestResult.Code,
                    message = message,
                    data = null
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

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
                {
                    success = false,
                    code = code, 
                    message = message,
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            if(request.data == null)
            {
                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
                {
                    success = false,
                    code = ApiCode.EmptyRequest, 
                    message = "请求错误",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }

            var notebookList = Services.VocabularyNotebookService.FindVocabularyNotebooksByTag(request.data.tag,userData.UserId,out var failures);

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

                return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{failures.errorCategory}] {failures.errorMessage}",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.VocabularyNotebookListSummaryResponse()
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

        public static async Task<IResult> CreateVocabularyNotebook(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.CreateVocabularyNotebookRequest>(context);

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
                    message = "请求错误",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }

            var failures = Services.VocabularyNotebookService.CreateVocabularyNotebook(request.data,userData.UserId);

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

                return Results.Json(new Models.Dtos.Responses.ApiResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{failures.errorCategory}] {failures.errorMessage}",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.ApiResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }

        public static async Task<IResult> UpdateVocabularyNotebook(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.UpdateVocabularyNotebookRequest>(context);

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
                    message = "请求错误",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }

            var failures = Services.VocabularyNotebookService.UpdateVocabularyNotebook(request.data,userData.UserId);

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

                return Results.Json(new Models.Dtos.Responses.ApiResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{failures.errorCategory}] {failures.errorMessage}",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.ApiResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }

        public static async Task<IResult> DeleteVocabularyNotebook(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.DeleteVocabularyNotebookRequest>(context);

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
                    message = "请求错误",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }

            var failures = Services.VocabularyNotebookService.DeleteVocabularyNotebook(request.data,userData.UserId);

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

                return Results.Json(new Models.Dtos.Responses.ApiResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{failures.errorCategory}] {failures.errorMessage}",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.ApiResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }
        
        public static async Task<IResult> DeleteVocabularyNotebookByList(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.DeleteVocabularyNotebooksRequest>(context);

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

                return Results.Json(new Models.Dtos.Responses.DeleteVocabularyNotebookByListSummaryResponse()
                {
                    success = false,
                    code = requestResult.Code,
                    message = message,
                    data = null
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

                return Results.Json(new Models.Dtos.Responses.DeleteVocabularyNotebookByListSummaryResponse()
                {
                    success = false,
                    code = code, 
                    message = message,
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            if(request.data == null)
            {
                return Results.Json(new Models.Dtos.Responses.DeleteVocabularyNotebookByListSummaryResponse()
                {
                    success = false,
                    code = ApiCode.EmptyRequest, 
                    message = "请求错误",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }
        
            var failures = Services.VocabularyNotebookService.DeleteNotebooks(request.data,userData.UserId);

            if(failures.Count() > 0)
            {
                return Results.Json(new Models.Dtos.Responses.DeleteVocabularyNotebookByListSummaryResponse()
                {
                    success = false,
                    code = ApiCode.InternalError,
                    message = $"一共有:{failures.Count()} 个错误",
                    data = new()
                    {
                        responseDataList = Models.Dtos.Responses.Auxiliary.VocabularyNotebookOperationFailureListToResponseData(failures)
                    }
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.InternalError));
            }

            return Results.Json(new Models.Dtos.Responses.DeleteVocabularyNotebookByListSummaryResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
                data = null
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }
    }
}