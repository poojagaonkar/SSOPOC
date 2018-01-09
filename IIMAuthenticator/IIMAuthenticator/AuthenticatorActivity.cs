using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Accounts;
using Android.Util;
using System.Threading.Tasks;
using IIMAuthenticator;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace IIMAuthenticator
{
    [Activity(Label = "AuthenticatorActivity", MainLauncher = true)]
    public class AuthenticatorActivity : AccountAuthenticatorActivity
    {
        public  static string ARG_ACCOUNT_TYPE = "ACCOUNT_TYPE";
        public  static string ARG_AUTH_TYPE = "AUTH_TYPE";
        public  static string ARG_ACCOUNT_NAME = "ACCOUNT_NAME";
        public  static string ARG_IS_ADDING_NEW_ACCOUNT = "IS_ADDING_ACCOUNT";

        public static  string KEY_ERROR_MESSAGE = "ERR_MSG";

        public  static string PARAM_USER_PASS = "USER_PASS";

        private  int REQ_SIGNUP = 1;

        private string TAG = "AuthenticatorActivity";

        private AccountManager mAccountManager;
        private string mAuthTokenType;


        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            mAccountManager = AccountManager.Get(BaseContext);

            string accountName = Intent.GetStringExtra(ARG_ACCOUNT_NAME);
            mAuthTokenType = Intent.GetStringExtra(ARG_ACCOUNT_TYPE);

            if(mAuthTokenType == null)
            {
                mAuthTokenType = AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS;

            }
            //if(accountName != null)
            //{
            //    FindViewById<TextView>(Resource.Id.accountName).Text = accountName;
            //}

            var resultIntent = await Submit();

            if (resultIntent.HasExtra(KEY_ERROR_MESSAGE))
            {
                Toast.MakeText(this, Intent.GetStringExtra(KEY_ERROR_MESSAGE), ToastLength.Long).Show();
            }
            else
            {
                FinishLogin(resultIntent);
            }
        }

        public async Task<Intent> Submit()
        {
            Log.Debug("IIM", TAG + "> Started authenticating");
           
            Bundle data = new Bundle();
            try
            {
               var result = await ParseComServerAuthenticate.Instance.UserSignIn("https://graph.microsoft.com", new PlatformParameters(this));

                /*Add the user info from UserInfo object*/
                data.PutString(AccountManager.KeyAccountName, result.AuthResult.UserInfo.GivenName + " " + result.AuthResult.UserInfo.FamilyName);
                data.PutString(AccountManager.KeyAccountType, result.AuthResult.AccessTokenType);
                data.PutString(AccountManager.KeyAuthtoken, result.AuthResult.AccessToken);
                //data.PutString(PARAM_USER_PASS, userPass);

            }
            catch (Exception e)
            {
                data.PutString(KEY_ERROR_MESSAGE, e.Message);
            }

            Intent res = new Intent();
            res.PutExtras(data);
            return res;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == REQ_SIGNUP && resultCode == Result.Ok)
            {
                FinishLogin(data);
            }
            else
                base.OnActivityResult(requestCode, resultCode, data);
        }

        private void FinishLogin(Intent data)
        {
            Log.Debug("udinic", TAG + "> finishLogin");

            String accountName = Intent.GetStringExtra(AccountManager.KeyAccountName);
            String accountPassword = Intent.GetStringExtra(PARAM_USER_PASS);
            Account account = new Account(accountName, Intent.GetStringExtra(AccountManager.KeyAccountType));

            if (Intent.GetBooleanExtra(ARG_IS_ADDING_NEW_ACCOUNT, false))
            {
                Log.Debug("udinic", TAG + "> finishLogin > addAccountExplicitly");
                String authtoken = Intent.GetStringExtra(AccountManager.KeyAuthtoken);
                String authtokenType = mAuthTokenType;

                // Creating the account on the device and setting the auth token we got
                // (Not setting the auth token will cause another call to the server to authenticate the user)
                mAccountManager.AddAccountExplicitly(account, accountPassword, null);
                mAccountManager.SetAuthToken(account, authtokenType, authtoken);
            }
            else
            {
                Log.Debug("IIM", TAG + "> finishLogin > setPassword");
                mAccountManager.SetPassword(account, accountPassword);
            }

            SetAccountAuthenticatorResult(Intent.Extras);
            SetResult(Result.Ok, Intent);
            Finish();
        }
    }
}