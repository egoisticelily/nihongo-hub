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
    }
}