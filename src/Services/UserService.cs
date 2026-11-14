using NihongoHub.Models.Entities;
using NihongoHub.Models.Code;

namespace NihongoHub.Services
{
    public static class UserService
    {
        public static bool ValidateUserIdentity(TokenUserData userData,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            if (!Data.UserRepository.FindUserById(userData.UserId,out var user,out errorMessage,out errorCategory))
            {
                errorMessage = $"[ERROR]服务器发生错误，请您联系管理员 \n - {errorMessage}";
                return false;
            }
            if(user == null)
            {
                errorMessage = $"[INFO]该用户不存在";
                errorCategory = Logging.LogCategory.None;
                return false;
            }

            if (!user.enabled)
            {
                errorMessage = $"[INFO]该用户被禁用";
                errorCategory = Logging.LogCategory.None;
                return false;
            }

            if(user.usertype != userData.Role)
            {
                errorMessage = $"[INFO]该用户身份不匹配";
                errorCategory = Logging.LogCategory.None;
                return false;
            }

            return true;
        }
        
        public static bool UserVerification(string userName,string passWord,out int? userId,out UserType? userType,out string errorMessage,out Logging.LogCategory errorCategory,out int code)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;
            userId = null; userType = null; 

            if(!Data.UserRepository.FindUserByUsername(userName,out var user,out errorMessage,out errorCategory))
            {
                code = ApiCode.DatabaseError;
                return false;
            }

            if(user == null)
            {
                errorMessage = "用户名或密码错误";
                errorCategory = Logging.LogCategory.None;
                code = ApiCode.EmptyRequest;
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(passWord, user.passwordhash))
            {
                errorMessage = "用户名或密码错误";
                errorCategory = Logging.LogCategory.None;
                code = ApiCode.EmptyRequest;
                return false;
            }

            if (!user.enabled)
            {
                errorMessage = "用户名或密码错误";
                errorCategory = Logging.LogCategory.None;
                code = ApiCode.EmptyRequest;
                return false;
            }
            
            code = ApiCode.Success;
            userId = user.id; userType = user.usertype;
            return true;
        }

    }
}