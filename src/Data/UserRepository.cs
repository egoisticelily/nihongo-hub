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
        public static List<Models.Entities.User> GetUsersAll()
        {
            //由于这是初始化时调用的，一旦出现错误，我们可以立刻结束进程而不是继续运行
            try
            {
                return Init.DatabaseInit.Current.Users.FindAll().ToList();
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Fatal("[Fatal]数据库出现错误 \n 请管理员自行排除",Logging.LogCategory.Database,ex);
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Fatal("[Fatal]调用数据库时出现未知错误 \n 请管理员自行排除",Logging.LogCategory.Unknown,ex);
            }
            return new();
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

        public static bool Delete(int id,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                if (!Init.DatabaseInit.Current.Users.Delete(id))
                {
                    errorMessage = "未能删除用户(可能是用户不存在)";
                    errorCategory = Logging.LogCategory.Database;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"删除用户时数据库错误",ex);

                errorMessage="删除用户时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Unknown,Logging.LogLevel.Error,"删除用户时发生未知错误",ex);

                errorMessage="删除用户时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
        public static bool DeleteByUsername(string username,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            if(!FindUserByUsername(username,out var user,out errorMessage,out errorCategory))
            {
                return false;
            }

            if(user == null)
            {
                errorMessage = "用户不存在";
                errorCategory = Logging.LogCategory.None;
                return false;
            }

            if(!Delete(user.id,out errorMessage,out errorCategory))
            {
                return false;
            }

            return true;
        }
    }
}