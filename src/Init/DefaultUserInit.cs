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

            if(userAll.Count() == 0)
            {
                if(!Services.UserService.CreateDefaultUser(out var userName,out var password,out var errorMessage,out var errorCategory))
                {
                    Logging.LogReporter.Fatal($"[Fatal][{errorCategory}]创建用户时出现严重错误 -\n-{errorMessage}");
                }

                if(!Utilities.FileUtility.WriteFile($"---DefaultAdmin---\n\nusername: {userName}\npassword: {password}","Config","initial-admin-credentials.txt",out errorMessage,out errorCategory))
                {
                    Logging.LogReporter.Report(errorCategory,Logging.LogLevel.Error,$"[创建文件时] {errorMessage}");
                    return;
                }

                Logging.LogReporter.Secret($"[SECRET] 默认用户创建成功 文件已写入: Config/initial-admin-credentials.txt\n username: {userName}\n password: {password}");
                
                return;
            }
            return;
        }
    }
}