using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace NihongoHub.Logging
{
    public enum LogCategory
    {
        Json,       //JSON序列、反序列时遇到的问题
        File,       //文件相关
        Database,   //数据库异常
        Http,       //HTTP相关错误
        Unknown,    //泛指一切未收录的异常
        None        //不标注类别(普通业务日志)
    }

    public enum LogLevel
    {
        Info,
        Warn,
        Error,
        Fatal
    }

    public static class LogReporter
    {
        public static void Report(LogCategory category, LogLevel level, string message, Exception? exception = null)
        {
            if (level == LogLevel.Fatal)
            {
                Fatal(message, category, exception);
            }
            Write(level, Format(category, message), exception);
        }

        public static void Report(LogLevel level, string message, Exception? exception = null)
        {
            Report(LogCategory.None, level, message, exception);
        }

        //致命错误:记录、刷新、结束程序。[DoesNotReturn] 让编译器知道调用之后不会继续
        [DoesNotReturn]
        public static void Fatal(string message, LogCategory category = LogCategory.None, Exception? exception = null)
        {
            Write(LogLevel.Fatal, Format(category, message), exception);
            Log.CloseAndFlush();
            Environment.Exit(1);
        }

        public static void Secret(string message)
        {
            Console.WriteLine(message);
        }

        private static string Format(LogCategory category, string message)
        {
            return category == LogCategory.None ? message : $"[{category}] {message}";
        }

        private static void Write(LogLevel level, string text, Exception? exception)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Log.Information(exception, "{Message}", text);
                    break;
                case LogLevel.Warn:
                    Log.Warning(exception, "{Message}", text);
                    break;
                case LogLevel.Error:
                    Log.Error(exception, "{Message}", text);
                    break;
                case LogLevel.Fatal:
                    Log.Fatal(exception, "{Message}", text);
                    break;
            }
        }
    }
}