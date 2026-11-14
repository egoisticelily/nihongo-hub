namespace NihongoHub
{
    public static class Program
    {
        public static void Main()
        {
            //这里执行初始化
            
            Init.LoggingInit.Init(); //先进行Log初始化
            Init.ConfigInit.Load(); //config初始化
            Init.DatabaseInit.Init(); //数据库初始化
            Init.JwtInit.Init(); //Jwt初始化
            Init.DefaultUserInit.Init(); //默认用户初始化
            Init.ServerInit.Setup(); //服务端初始化
        }
    }
}