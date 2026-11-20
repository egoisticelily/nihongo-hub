using System.Text.Json;
using NihongoHub.Models.Code;
using System.Diagnostics.CodeAnalysis;

namespace NihongoHub.API.Utilities
{
    public class RequestValidationResult<T>
        where T: class
    {
        /// <summary>
        /// 禁止在code为Success时传入nullrequest
        /// </summary>
        /// <param name="code"></param>
        /// <param name="request"></param>
        public RequestValidationResult(int code,T? request)
        {
            if(code == ApiCode.Success && request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            Code = code;
            Request = request;
        }
        
        [MemberNotNullWhen(true,nameof(Request))]
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
        public static async Task<RequestValidationResult<T>> ValidateJsonRequestAsync<T>(HttpContext context) 
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