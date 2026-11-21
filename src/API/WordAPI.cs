using NihongoHub.Models.Code;

namespace NihongoHub.API
{
    public static class WordAPI
    {
        public static async Task<IResult> SearchWordByQuery(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.SearchWordsByQueryRequest>(context);

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

                return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
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

                return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
                {
                    success = false,
                    code = code, 
                    message = message,
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            if(request.data == null)
            {
                return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
                {
                    success = false,
                    code = ApiCode.EmptyRequest, 
                    message = "请求错误",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }

            var wordList = Services.WordService.FindWordsByQuery(request.data.query,userData.UserId,out var failure);

            if(failure != null)
            {
                switch (failure.errorCategory)
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

                return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{failure.errorCategory}] {failure.errorMessage}",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
                data = API.WordValidator.DatabaseWordEntitiesToResponseByList(wordList)
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }

        public static async Task<IResult> SearchWordByVocabularyNotebookId(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.SearchWordsByVocabularyNotebookIdRequest>(context);

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

                return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
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

                return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
                {
                    success = false,
                    code = code, 
                    message = message,
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            if(request.data == null)
            {
                return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
                {
                    success = false,
                    code = ApiCode.EmptyRequest, 
                    message = "请求错误",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }

            var wordList = Services.WordService.FindWordsByNotebookId(request.data.id,userData.UserId,out var failure);

            if(failure.Count > 0)
            {
                switch (failure[0].errorCategory)
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

                return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
                {
                    success = false,
                    code = code, 
                    message = $"[{failure[0].errorCategory}] {failure[0].errorMessage}",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.SearchWordsResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
                data = API.WordValidator.DatabaseWordEntitiesToResponseByList(wordList)
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }

        public static async Task<IResult> CreateWord(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.CreateWordRequest>(context);

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

            var failure = Services.WordService.CreateWord(request.data,userData.UserId);

            if(failure != null)
            {
                switch (failure.errorCategory)
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
                    message = $"[{failure.errorCategory}] {failure.errorMessage}",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.ApiResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }

        public static async Task<IResult> UpdateWord(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.UpdateWordRequest>(context);

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

            var failure = Services.WordService.UpdateWord(request.data,userData.UserId);

            if(failure != null)
            {
                switch (failure.errorCategory)
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
                    message = $"[{failure.errorCategory}] {failure.errorMessage}",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.ApiResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }

        public static async Task<IResult> DeleteWord(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.DeleteWordRequest>(context);

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

            var failure = Services.WordService.DeleteWord(request.data,userData.UserId);

            if(failure != null)
            {
                switch (failure.errorCategory)
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
                    message = $"[{failure.errorCategory}] {failure.errorMessage}",
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            return Results.Json(new Models.Dtos.Responses.ApiResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }

        public static async Task<IResult> DeleteWordbyList(HttpContext context)
        {
            var requestResult = await Utilities.JsonRequestValidator.ValidateJsonRequestAsync<Models.Dtos.Requests.DeleteWordsRequest>(context);

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

                return Results.Json(new Models.Dtos.Responses.DeleteWordsByListSummaryResponse()
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

                return Results.Json(new Models.Dtos.Responses.DeleteWordsByListSummaryResponse()
                {
                    success = false,
                    code = code, 
                    message = message,
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(code));
            }

            if(request.data == null)
            {
                return Results.Json(new Models.Dtos.Responses.DeleteWordsByListSummaryResponse()
                {
                    success = false,
                    code = ApiCode.EmptyRequest, 
                    message = "请求错误",
                    data = null
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.EmptyRequest));
            }

            var failure = Services.WordService.DeleteWords(request.data,userData.UserId);

            if(failure.Count() > 0)
            {
                return Results.Json(new Models.Dtos.Responses.DeleteWordsByListSummaryResponse()
                {
                    success = false,
                    code = ApiCode.InternalError,
                    message = $"一共有:{failure.Count()} 个错误",
                    data = new()
                    {
                        responseDataList = Models.Dtos.Responses.Auxiliary.WordOperationFailureListToResponseData(failure)
                    }
                },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.InternalError));
            }

            return Results.Json(new Models.Dtos.Responses.DeleteWordsByListSummaryResponse()
            {
                success = true,
                code = ApiCode.Success, 
                message = "OK",
                data = null
            },statusCode:Models.Code.ApiCode.GetCorrespondingHttpcode(ApiCode.Success));
        }
    }
}