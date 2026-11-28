using Serilog;

namespace NihongoHub.Init
{
    public static class LoggingInit
    {
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
