using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace IIMIosAuthenticator
{
    public class AuthContainer
    {
        public AuthenticationResult AuthResult { get; set; }
        public bool IsException { get; set; }
        public string Message { get; set; }
        public AuthResultCode ResultCode { get; set; }
        public string SharePointAccessToken { get; set; }
        public string GraphAccessToken
        {
            get
            {
                if (null == AuthResult)
                    return string.Empty;
                else return AuthResult.AccessToken;
            }
        }
    }
}