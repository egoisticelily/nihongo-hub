using LiteDB;

namespace NihongoHub.Data
{
    public static class UserRepository
    {
        public static Models.Entities.User? FindUserByUsername(string username)
        {
            return Init.DatabaseInit.Current.Users.FindOne(x => x.username == username);
        }

        public static Models.Entities.User? FindUserById(int userId)
        {
            return Init.DatabaseInit.Current.Users.FindById(userId);
        }

        ///这里加hint是为了能够返回一些在操作时暴露的问题
        /// hint可以为空

        public static bool Insert(Models.Entities.User user,ref string errorMessage,ref Logging.LogCategory errorCategory)
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

        public static bool Update(Models.Entities.User user,ref string errorMessage,ref Logging.LogCategory errorCategory)
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