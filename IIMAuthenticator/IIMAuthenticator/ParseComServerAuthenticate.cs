using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IIMAuthenticator
{
    public class ParseComServerAuthenticate : IServerAuthenticate
    {
        private static ParseComServerAuthenticate instance = null;
        private AuthenticationResult authResult;
        private AuthenticationContext authContext;
        private string spResourceId;

        public AuthContainer AuthenticationResult { get; set; }


        public ParseComServerAuthenticate()
        {
            AuthenticationResult = null;
            authContext = new AuthenticationContext(ServiceConstants.AUTHORITY);
        }

        public static ParseComServerAuthenticate Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ParseComServerAuthenticate();
                }
                return instance;
            }
        }


        public async Task<AuthContainer> UserSignIn(string resource, IPlatformParameters platformParameters)
        {
            Uri SPUri = new Uri(ServiceConstants.SHAREPOINTURL);


            AuthenticationResult = new AuthContainer();
            try
            {
                if (!string.IsNullOrEmpty(SPUri.Scheme) && !string.IsNullOrEmpty(SPUri.Host))
                {
                    spResourceId = SPUri.Scheme + "://" + SPUri.Host;

                    if (authContext != null && authContext.TokenCache.ReadItems().Any())
                        
                        authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);
                    }
                    else
                    {
                        authContext = new AuthenticationContext(ServiceConstants.AUTHORITY);

                    }

                    var authResult = await authContext.AcquireTokenAsync(resource, ServiceConstants.CLIENTID, ServiceConstants.RETURNURI, platformParameters);

                    if (null == authResult)
                    {
                        AuthenticationResult.ResultCode = AuthResultCode.Unknown;
                    }
                    else
                    {
                        AuthenticationResult.ResultCode = AuthResultCode.Success;
                        AuthenticationResult.AuthResult = authResult;

                        var expiry = DateTime.Parse(authResult.ExpiresOn.UtcDateTime.ToString("u"),
                                                            System.Globalization.CultureInfo.InvariantCulture);

                        //var sharePointResult = await authContext.AcquireTokenAsync(ServiceConstants.SHAREPOINTURL, ServiceConstants.CLIENTID, ServiceConstants.RETURNURI, platformParameters);

                        var sharePointResult = await authContext.AcquireTokenSilentAsync(spResourceId , ServiceConstants.CLIENTID, new UserIdentifier(authResult.UserInfo.UniqueId, UserIdentifierType.UniqueId));
                        AuthenticationResult.SharePointAccessToken = sharePointResult.AccessToken;

                    }
                }
                 catch (AdalException adalEx)
                {
                    AuthenticationResult.Message = adalEx.ErrorCode;
                    switch (adalEx.ErrorCode)
                    {
                        case "authentication_canceled":
                            AuthenticationResult.ResultCode = AuthResultCode.Canceled;
                            break;
                        case "access_denied":
                            AuthenticationResult.ResultCode = AuthResultCode.AccessDenied;
                            break;
                        case "failed_to_acquire_token_silently":
                            authContext = null;
                            authContext = new AuthenticationContext(ServiceConstants.AUTHORITY);
                            var res = await UserSignIn(resource, platformParameters);

                            break;
                        default:
                            AuthenticationResult.ResultCode = AuthResultCode.Unknown;
                        break;
                }
            }
            catch (Exception ex)
            {
                AuthenticationResult.ResultCode = AuthResultCode.Unknown;
                AuthenticationResult.Message = ex.Message;
            }

            return AuthenticationResult;
        }

        //public string UserSignUp(string name, string email, string pass, string authType)
        //{

        //}
    }
}