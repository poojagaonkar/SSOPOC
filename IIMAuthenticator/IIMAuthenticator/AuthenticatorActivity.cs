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
using IIMAuthenticator.AuthService;

namespace com.rapidcircle.iimAuthenticator
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
        private string mAccountType;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(IIMAuthenticator.Resource.Layout.Main);

            var btnSubmit = FindViewById<Button>(IIMAuthenticator.Resource.Id.btnSubmit);

            mAccountManager = AccountManager.Get(BaseContext);

            string accountName = Intent.GetStringExtra(ARG_ACCOUNT_NAME);
            mAuthTokenType = Intent.GetStringExtra(ARG_AUTH_TYPE);
            mAccountType = Intent.GetStringExtra(ARG_ACCOUNT_TYPE) ?? "com.rapidcircle.iimAuthenticator";

            if (mAuthTokenType == null)
            {
                mAuthTokenType = AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS;

            }

            //if(accountName != null)
            //{
            //    FindViewById<TextView>(Resource.Id.accountName).Text = accountName;
            //}

            btnSubmit.Click += async delegate {

                var resultIntent = await Submit();

                if (resultIntent.HasExtra(KEY_ERROR_MESSAGE))
                {
                    Toast.MakeText(this, "Error Occured" + Intent.GetStringExtra(KEY_ERROR_MESSAGE), ToastLength.Long).Show();

                }
                else
                {
                    //Toast.MakeText(this, "Success" + resultIntent.GetStringExtra(AccountManager.KeyAuthtoken), ToastLength.Long).Show();
                   
                    FinishLogin(resultIntent);
                }

            };
            
        }

        public async Task<Intent> Submit()
        {
            Log.Debug("IIM", TAG + "> Started authenticating");


           
            Bundle data = new Bundle();
            try
            {
                //var result = await ParseComServerAuthenticate.Instance.UserSignIn("https://graph.microsoft.com", new PlatformParameters(this));

                var result = await ADAuthService.Instance.Authenticate("https://graph.microsoft.com", new PlatformParameters(this));


                /*Add the user info from UserInfo object*/
                data.PutString(AccountManager.KeyAccountName, result.AuthResult.UserInfo.GivenName + " " + result.AuthResult.UserInfo.FamilyName);
                data.PutString(AccountManager.KeyAccountType, mAccountType);
                data.PutString(AccountManager.KeyAuthtoken, result.AuthResult.AccessToken);
                data.PutBoolean(ARG_IS_ADDING_NEW_ACCOUNT, true);
                //data.PutExtra(AccountManager.KeyAccountAuthenticatorResponse, response);
                //data.PutString(PARAM_USER_PASS, userPass);

            }
            catch (Exception e)
            {
                data.PutString(KEY_ERROR_MESSAGE, e.StackTrace);
            }

            Intent res = new Intent();
            res.PutExtras(data);
            return res;
        }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (resultCode == Result.Ok)
                {
                    AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
                    //DialogHelper.ShowProgressDialog(this, false, "Logging you in..");
                }
                else
                {
                   // DialogHelper.ShowProgressDialog(this, true, "Logging you in..");
                    return;
                }

            }
            catch (Exception ex)
            {

            }

        }
        //protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        //{
        //    if (requestCode == REQ_SIGNUP && resultCode == Result.Ok)
        //    {
        //        FinishLogin(data);
        //    }
        //    else
        //        base.OnActivityResult(requestCode, resultCode, data);
        //}

        private void FinishLogin(Intent data)
        {
            try
            {
                Log.Debug("IIM", TAG + "> finishLogin");

                String accountName = data.GetStringExtra(AccountManager.KeyAccountName);
                String accountToken = data.GetStringExtra(AccountManager.KeyAuthtoken);
                Account account = new Account(accountName, data.GetStringExtra(AccountManager.KeyAccountType));

                var status = data.GetBooleanExtra(ARG_IS_ADDING_NEW_ACCOUNT, false);
                if (status)
                {
                    Log.Debug("udinic", TAG + "> finishLogin > addAccountExplicitly");
                    String authtoken = data.GetStringExtra(AccountManager.KeyAuthtoken);
                    String authtokenType = mAuthTokenType;

                    // Creating the account on the device and setting the auth token we got
                    // (Not setting the auth token will cause another call to the server to authenticate the user)
                    mAccountManager.AddAccountExplicitly(account, accountToken, new Bundle());
                    mAccountManager.SetAuthToken(account, authtokenType, authtoken);

                    Toast.MakeText(this, "Account added", ToastLength.Long).Show();
                }
                else
                {
                    Log.Debug("IIM", TAG + "> finishLogin > setPassword");

                  
                    //mAccountManager.SetPassword(account, accountToken);
                    Toast.MakeText(this, accountName + accountToken, ToastLength.Long).Show();

                }
                var mIntent = new Intent();
                mIntent.PutExtra(AccountManager.KeyAccountName, accountName);
                mIntent.PutExtra(AccountManager.KeyAccountType, data.GetStringExtra(AccountManager.KeyAccountType));
                SetAccountAuthenticatorResult(mIntent.Extras);
                SetResult(Result.Ok, data);
                Finish();
            }
            catch(Exception xe)
            {
                Toast.MakeText(this, xe.StackTrace, ToastLength.Long).Show();
            }
        }
    }
}