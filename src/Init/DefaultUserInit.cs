using NihongoHub.Models.Entities;

namespace NihongoHub.Init
{
    public static class DefaultUserInit
    {
        public static void Init()
        {
            AnyUserCheck();
        }
        public static void AnyUserCheck()
        {
            //将来写一个负责写入文件的方法，这里先只写入Log
            var userAll = Data.UserRepository.GetUsersAll();

            if(userAll == null)
            {
                //一般不会走到这
                if(!Services.UserService.CreateDefaultUser(out var userName,out var password))
                {
                    Logging.LogReporter.Report(Logging.LogLevel.Fatal,$"默认用户创建失败，详细请看Log");
                }

                Logging.LogReporter.Report(Logging.LogLevel.Info,$"默认用户创建成功 -\n-username: {userName}  password: {password}");
                return;
            }
        }
    }
}