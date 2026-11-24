using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace NihongoHub.Init{
    public static class ServerInit
    {
        public static void Setup()
        {
            var app = CreateApp();

            ConfigureRoutes(app);
            try
            {
                app.Run();
            }
            catch(Exception ex)
            {
                NihongoHub.Logging.LogReporter.Fatal("启动HTTP服务端时出现致命问题 \n 可能是Cert配置问题",Logging.LogCategory.Http,ex);
            }
        }

        private static WebApplication CreateApp()
        {
            //创建一个WebApplicationBuilder实例
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                //指定目录
                ContentRootPath = AppContext.BaseDirectory,
                WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
            });

            ConfigureWebHost(builder);
            return builder.Build();

            //构建
            static void ConfigureWebHost(WebApplicationBuilder builder) {
                builder.WebHost.ConfigureKestrel(ConfigureKestrel);
            }

            //监听端口配置
            static void ConfigureKestrel(KestrelServerOptions options) {
                var server = NihongoHub.Init.ConfigInit.Current.Server;
                

                if (!System.Net.IPAddress.TryParse(server.Host, out var address))
                {
                    NihongoHub.Logging.LogReporter.Fatal("提供的Host不合法",Logging.LogCategory.Http);
                }

                if (!server.HttpsEnable)
                {
                    options.Listen(address,server.Port);
                    return;
                }

                options.Listen(address,server.Port,listenOptions =>
                {
                    ConfigureHttps(listenOptions, server);
                });
                
            }
            //https配置
            static void ConfigureHttps(ListenOptions listenOptions,NihongoHub.Models.Config.ServerConfig server) {
                listenOptions.UseHttps(server.PfxPath!, server.PfxPassword);
            }
        }

        private static void ConfigureRoutes(WebApplication app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            //Auth
            app.MapPost("/api/v1/auth/login",(Func<HttpContext,Task<IResult>>)API.AuthAPI.Login);

            //VocabularyNotebook
            app.MapPost("/api/v1/vocabulary-notebooks/list",(Func<HttpContext,Task<IResult>>)API.VocabularyNotebookAPI.SearchAllVocabularyNotebook);

            app.MapPost("/api/v1/vocabulary-notebooks/search-by-tag",(Func<HttpContext,Task<IResult>>)API.VocabularyNotebookAPI.SearchVocabularyNotebookByTag);

            app.MapPost("/api/v1/vocabulary-notebooks/create",(Func<HttpContext,Task<IResult>>)API.VocabularyNotebookAPI.CreateVocabularyNotebook);

            app.MapPost("/api/v1/vocabulary-notebooks/update",(Func<HttpContext,Task<IResult>>)API.VocabularyNotebookAPI.UpdateVocabularyNotebook);

            app.MapPost("/api/v1/vocabulary-notebooks/delete",(Func<HttpContext,Task<IResult>>)API.VocabularyNotebookAPI.DeleteVocabularyNotebook);

            app.MapPost("/api/v1/vocabulary-notebooks/delete-batch",(Func<HttpContext,Task<IResult>>)API.VocabularyNotebookAPI.DeleteVocabularyNotebookByList);

            //Word
            app.MapPost("/api/v1/words/search",(Func<HttpContext,Task<IResult>>)API.WordAPI.SearchWordByQuery);

            app.MapPost("/api/v1/words/search-by-notebook",(Func<HttpContext,Task<IResult>>)API.WordAPI.SearchWordByVocabularyNotebookId);

            app.MapPost("/api/v1/words/create",(Func<HttpContext,Task<IResult>>)API.WordAPI.CreateWord);

            app.MapPost("/api/v1/words/update",(Func<HttpContext,Task<IResult>>)API.WordAPI.UpdateWord);

            app.MapPost("/api/v1/words/delete",(Func<HttpContext,Task<IResult>>)API.WordAPI.DeleteWord);

            app.MapPost("/api/v1/words/delete-batch",(Func<HttpContext,Task<IResult>>)API.WordAPI.DeleteWordbyList);

            //StatusAPI
            app.MapPost("/api/v1/status/heartbeat-detection",(Func<HttpContext,Task<IResult>>)API.StatusAPI.HeartbeatDetection);
        }
    }
}