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
            List<Models.Entities.User> userAll = Data.UserRepository.GetUsersAll();

            if(userAll.Count() < 1)
            {
                if(!Services.UserService.CreateDefaultUser(out var userName,out var password,out var errorMessage,out var errorCategory))
                {
                    Logging.LogReporter.Fatal($"[Fatal][{errorCategory}]创建用户时出现严重错误 -\n-{errorMessage}");
                }

                Logging.LogReporter.Report(Logging.LogLevel.Info,$"默认用户创建成功 -\n-username: {userName}  password: {password}");
                return;
            }
            return;
        }
    }
}