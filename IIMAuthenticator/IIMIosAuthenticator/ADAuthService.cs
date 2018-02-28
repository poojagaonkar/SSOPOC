using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using com.rapidcircle.iimAuthenticator;
using System.Threading.Tasks;

namespace IIMAuthenticator.AuthService
{
    public class ADAuthService
    {
        private AuthenticationContext authContext;

        public AuthContainer AuthenticationResult { get; set; }


        private ADAuthService()
        {
            AuthenticationResult = null;
            authContext = new AuthenticationContext(ServiceConstants.AUTHORITY);
        }

        private static ADAuthService instance = null;
        private AuthenticationResult authResult;

        public static ADAuthService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ADAuthService();
                }
                return instance;
            }
        }

        public async Task<AuthContainer> Authenticate(string resource, IPlatformParameters platformParameters)
        {
            Uri SPUri = new Uri(ServiceConstants.SHAREPOINTURL);


            AuthenticationResult = new AuthContainer();
            try
            {
                if (!string.IsNullOrEmpty(SPUri.Scheme) && !string.IsNullOrEmpty(SPUri.Host))
                {
                    string spResourceId = SPUri.Scheme + "://" + SPUri.Host;

                    if (authContext != null && authContext.TokenCache.ReadItems().Any())
                    {
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

                        var sharePointResult = await authContext.AcquireTokenSilentAsync(spResourceId, ServiceConstants.CLIENTID, new UserIdentifier(authResult.UserInfo.UniqueId, UserIdentifierType.UniqueId));
                        AuthenticationResult.SharePointAccessToken = sharePointResult.AccessToken;

                    }
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
                        var res = await Authenticate(resource, platformParameters);

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

        public async Task<AuthContainer> RenewToken(string resourceUri, string accessToken, IPlatformParameters param)
        {
            try
            {
                string siteUrl = ServiceConstants.SHAREPOINTURL;
                Uri SPUri = new Uri(siteUrl);
                string spResourceId = SPUri.Scheme + "://" + SPUri.Host;
                AuthenticationResult = new AuthContainer();

                //Pass old accessToken
                var userAssertion = new UserAssertion(accessToken);

                if (authContext == null)
                {
                    authContext = new AuthenticationContext(ServiceConstants.AUTHORITY);

                    authResult = await authContext.AcquireTokenAsync(resourceUri, ServiceConstants.CLIENTID, ServiceConstants.RETURNURI, param, new UserIdentifier(ServiceConstants.UserEmail, UserIdentifierType.RequiredDisplayableId));

                }
                else
                {
                    authResult = await authContext.AcquireTokenAsync(resourceUri, ServiceConstants.CLIENTID, userAssertion);

                }

                var userInfo = authResult.UserInfo;
                var apiAccessToken = authResult.AccessToken;
                var expiry = DateTime.Parse(authResult.ExpiresOn.UtcDateTime.ToString("u"),
                                       System.Globalization.CultureInfo.InvariantCulture);

                if (null == authResult)
                {
                    AuthenticationResult.ResultCode = AuthResultCode.Unknown;
                }
                else
                {
                    AuthenticationResult.ResultCode = AuthResultCode.Success;
                    AuthenticationResult.AuthResult = authResult;


                    var sharePointResult = await authContext.AcquireTokenSilentAsync(spResourceId, ServiceConstants.CLIENTID, new UserIdentifier(authResult.UserInfo.UniqueId, UserIdentifierType.UniqueId));
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
                    default:
                        AuthenticationResult.ResultCode = AuthResultCode.Unknown;
                        break;
                }

                authContext = null;
                await RenewToken(resourceUri, accessToken, param);
            }
            catch (Exception ex)
            {
                AuthenticationResult.ResultCode = AuthResultCode.Unknown;
                AuthenticationResult.Message = ex.Message;


            }

            return AuthenticationResult;
        }
        public bool Logout(bool completeLogout)
        {

            try
            {

                if (authContext != null)
                {
                    if (completeLogout)
                    {
                        authContext.TokenCache.Clear();
                        authContext = null;
                        AuthenticationResult = null;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {

                return
                    false;
            }
        }
    }
}