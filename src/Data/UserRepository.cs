using LiteDB;
using NihongoHub.Models.Entities;

namespace NihongoHub.Data
{
    public static class UserRepository
    {
        public static bool FindUserByUsername(string username,out Models.Entities.User? user,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;
            user = null;

            try
            {
                user = Init.DatabaseInit.Current.Users.FindOne(x => x.username == username);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询用户时数据库错误",ex);

                errorMessage="查询用户时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询用户时发生未知错误",ex);

                errorMessage="查询用户时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
        public static Models.Entities.User? FindUserByUsername(string username)
        {
            _ = FindUserByUsername(username,out var user,out _,out _);
            return user;
        }

        public static bool FindUserById(int userId,out Models.Entities.User? user,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;
            user = null;

            try
            {
                user = Init.DatabaseInit.Current.Users.FindById(userId);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询用户时数据库错误",ex);

                errorMessage="查询用户时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询用户时发生未知错误",ex);

                errorMessage="查询用户时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
        public static Models.Entities.User? FindUserById(int userId)
        {
            _ = FindUserById(userId,out var user,out _,out _);
            return user;
        }

        public static bool Insert(Models.Entities.User user,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            //先清空hint
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                Init.DatabaseInit.Current.Users.Insert(user);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]插入用户时数据库错误",ex);

                errorMessage="插入用户时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]插入用户时发生未知错误",ex);

                errorMessage="插入用户时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }

        public static bool Update(Models.Entities.User user,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                if (!Init.DatabaseInit.Current.Users.Update(user))
                {
                    //严格来讲，这里不属于报错
                    errorMessage = "未能更改用户数据(可能是用户不存在)";
                    errorCategory = Logging.LogCategory.Database;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]修改用户时数据库错误",ex);

                errorMessage="修改用户时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]修改用户时发生未知错误",ex);

                errorMessage="修改用户时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
    }
}