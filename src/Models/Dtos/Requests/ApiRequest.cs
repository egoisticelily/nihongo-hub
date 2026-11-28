///通用请求基类及公共请求结构

namespace NihongoHub.Models.Dtos.Requests
{
    public class ApiRequest<TData>
    {
        public string? token {get;set;}
        public TData? data {get;set;}
    }
    public class ApiRequest
    {
        public string? token {get;set;}
    }
}