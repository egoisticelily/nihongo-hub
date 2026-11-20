using System.Text.Json;
using NihongoHub.Models.Code;

namespace NihongoHub.API.Utilities
{
    public class RequestValidationResult<T>
    {
        public RequestValidationResult(int code,T? request)
        {
            Code = code;
            Request = request;
        }
        public bool Success
        {
            get
            {
                return Code == ApiCode.Success;
            }
        }
        public int Code {get;}
        public T? Request {get;}
    }
    public static class JsonRequestValidator
    {
        public async static Task<RequestValidationResult<T>> ValidateJsonRequestAsync<T>(HttpContext context) 
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