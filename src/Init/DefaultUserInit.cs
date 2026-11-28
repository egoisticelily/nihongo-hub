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
                    if(!Data.UserRepository.DeleteByUsername(userName,out errorMessage,out errorCategory))
                    {
                        Logging.LogReporter.Fatal($"[严重错误] 默认用户已经创建,但密码未能保存至文件.系统尝试归滚,但回滚也失败.\n如果您是管理员,建议您手动排除错误 {errorMessage}",errorCategory);
                        return;
                    }
                    Logging.LogReporter.Fatal($"[致命错误] 默认用户已经创建,但密码文件未能成功写入.系统目前已经回滚,您可以再次启动尝试.");
                    return;
                }

                Logging.LogReporter.Secret($"[SECRET] 默认用户创建成功 文件已写入: Config/initial-admin-credentials.txt");
                
                return;
            }
            return;
        }
    }
}