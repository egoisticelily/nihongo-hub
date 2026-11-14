namespace NihongoHub.Models.Code
{
    public static class ApiCode
    {
        public const int Success = 0;

        // 通用请求错误
        public const int InvalidRequest = 1000;
        public const int EmptyRequest = 1001;
        public const int InvalidJson = 1002;
        public const int MissingParameter = 1003;
        public const int InvalidParameter = 1004;
        public const int UnsupportedContentType = 1005;

        // 身份认证错误
        public const int AuthenticationFailed = 1100;
        public const int InvalidCredentials = 1101;
        public const int TokenMissing = 1102;
        public const int TokenInvalid = 1103;
        public const int TokenExpired = 1104;
        public const int TokenNotYetValid = 1105;
        public const int TokenClaimsInvalid = 1106;
        public const int UserIdentityInvalid = 1107;

        // 权限错误
        public const int PermissionDenied = 1200;
        public const int AdminRequired = 1201;
        public const int ResourceAccessDenied = 1202;

        // 服务器内部错误
        public const int InternalError = 9000;
        public const int DatabaseError = 9001;
        public const int UnknownError = 9002;
        public const int ConfigurationError = 9003;
        public const int InternalProcessingError = 9004;

        public static int GetCorrespondingHttpcode(int apiCode)
        {
            int code = 500;
            switch (apiCode)
            {
                case Success:
                    code = 200;
                    break;
                case InvalidRequest: case EmptyRequest: case InvalidJson: case MissingParameter: case InvalidParameter:
                    code = 400;
                    break;
                case UnsupportedContentType:
                    code = 415;
                    break;
                case AuthenticationFailed: case InvalidCredentials: case TokenMissing: case TokenInvalid: case TokenExpired: case TokenNotYetValid: case TokenClaimsInvalid: case UserIdentityInvalid:
                    code = 401;
                    break;
                case PermissionDenied: case AdminRequired: case ResourceAccessDenied:
                    code = 403;
                    break;
                case InternalError: case DatabaseError: case UnknownError: case ConfigurationError: case InternalProcessingError:
                    code = 500;
                    break;
                default:
                    code = 500;
                    break;
            }
            return code;
        }
    }
}