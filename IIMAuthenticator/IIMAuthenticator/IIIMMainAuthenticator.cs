using System;
using Android.Accounts;
using Android.OS;
using Android.Content;
using Android.Util;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace com.rapidcircle.iimAuthenticator
{
    public class IIIMMainAuthenticator : Android.Accounts.AbstractAccountAuthenticator
    {
        private String TAG = "IIMMainAuthenticator";
        private Context mContext;

        public IIIMMainAuthenticator(Context mContext) : base (mContext)
        {

            this.mContext = mContext;
        }

        public override Bundle AddAccount(AccountAuthenticatorResponse response, string accountType, string authTokenType, string[] requiredFeatures, Bundle options)
        {
            try
            {
                Log.Debug("IIM", TAG + "> addAccount");

            Intent intent = new Intent(mContext, typeof(AuthenticatorActivity));
            intent.PutExtra(AuthenticatorActivity.ARG_ACCOUNT_TYPE, accountType);
            intent.PutExtra(AuthenticatorActivity.ARG_AUTH_TYPE, authTokenType);
            intent.PutExtra(AuthenticatorActivity.ARG_IS_ADDING_NEW_ACCOUNT, true);
            intent.PutExtra(AccountManager.KeyAccountAuthenticatorResponse, response);

            Bundle bundle = new Bundle();
            bundle.PutParcelable(AccountManager.KeyIntent, intent);
            return bundle;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public override Bundle ConfirmCredentials(AccountAuthenticatorResponse response, Account account, Bundle options)
        {
            Console.WriteLine("Confirm Credentials");
            return this.ConfirmCredentials(response, account, options);
        }

        public override Bundle EditProperties(AccountAuthenticatorResponse response, string accountType)
        {
            throw new NotImplementedException();
        }

        public override Bundle GetAuthToken(AccountAuthenticatorResponse response, Account account, string authTokenType, Bundle options)
        {
            Log.Debug("IIM", TAG + "> GetAuthToken");

            // If the caller requested an authToken type we don't support, then
            // return an error
            if (!authTokenType.Equals(AccountGeneral.AUTHTOKEN_TYPE_READ_ONLY) && !authTokenType.Equals(AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS))
            {
                Bundle result = new Bundle();
                result.PutString(AccountManager.KeyErrorMessage, "invalid authTokenType");
                return result;
            }

            // Extract the username and password from the Account Manager, and ask
            // the server for an appropriate AuthToken.
            AccountManager am = AccountManager.Get(mContext);

            String authToken = am.PeekAuthToken(account, authTokenType);

            Log.Debug("IIM", TAG + "> peekAuthToken returned - " + authToken);

            // Lets give another try to authenticate the user
            //if (string.IsNullOrEmpty(authToken))
            //{
            //    string password = am.GetPassword(account);
            //    if (password != null)
            //    {
            //        try
            //        {
            //            Log.Debug("IIM", TAG + "> re-authenticating with the existing password");
            //            ParseComServerAuthenticate parseCom = new ParseComServerAuthenticate();
            //            authToken = parseCom.UserSignIn(ServiceConstants.GRAPHURI, new PlatformParameters(mContext));
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e.InnerException);
            //        }
            //    }
            //}

            // If we get an authToken - we return it
            if (!string.IsNullOrEmpty(authToken))
            {
                Bundle result = new Bundle();
                result.PutString(AccountManager.KeyAccountName, account.Name);
                result.PutString(AccountManager.KeyAccountType, account.Type);
                result.PutString(AccountManager.KeyAuthtoken, authToken);
                return result;
            }

            // If we get here, then we couldn't access the user's password - so we
            // need to re-prompt them for their credentials. We do that by creating
            // an intent to display our AuthenticatorActivity.
            Intent intent = new Intent(mContext, typeof(AuthenticatorActivity));
            intent.PutExtra(AccountManager.KeyAccountAuthenticatorResponse, response);
            intent.PutExtra(AuthenticatorActivity.ARG_ACCOUNT_TYPE, account.Type);
            intent.PutExtra(AuthenticatorActivity.ARG_AUTH_TYPE, authTokenType);
            intent.PutExtra(AuthenticatorActivity.ARG_ACCOUNT_NAME, account.Name);
            Bundle bundle = new Bundle();
            bundle.PutParcelable(AccountManager.KeyIntent, intent);
            return bundle;
        }

        public override string GetAuthTokenLabel(string authTokenType)
        {
            throw new NotImplementedException();
        }

        public override Bundle HasFeatures(AccountAuthenticatorResponse response, Account account, string[] features)
        {
            throw new NotImplementedException();
        }

        public override Bundle UpdateCredentials(AccountAuthenticatorResponse response, Account account, string authTokenType, Bundle options)
        {
            throw new NotImplementedException();
        }

   
    }
}