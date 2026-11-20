using NihongoHub.Models.Code;

namespace NihongoHub.API.Utilities
{
    public static class UsereValidator
    {
        public static bool ValidateUser(string? token,out Services.TokenUserData? userData,out int code)
        {
            if (String.IsNullOrWhiteSpace(token))
            {
                userData = null;
                code = ApiCode.TokenMissing;
                return false;
            }

            if(!Services.AuthService.ValidateToken(token,out userData,out _,out code))
            {
                return false;
            }

            if(userData == null)
            {
                code = ApiCode.UnknownError;
                return false;
            }

            if(!Services.UserService.ValidateUserIdentity(userData,out _,out _,out code))
            {
                return false;
            }

            return true;
        }
    }
}