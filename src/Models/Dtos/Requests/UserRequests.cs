namespace NihongoHub.Models.Dtos.Requests
{
    public class PasswordChangeRequestData
    {
        public required string password {get;set;}
    }
    public class PasswordChangeRequest : ApiRequest<PasswordChangeRequestData>;
}