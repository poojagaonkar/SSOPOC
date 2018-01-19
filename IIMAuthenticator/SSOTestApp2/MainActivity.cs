using Android.App;
using Android.Widget;
using Android.OS;
using Android.Accounts;
using IIMAuthenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Util;
using Java.Lang;
using Java.Util.Concurrent;
using com.rapidcircle.iimAuthenticator;
using System.Threading.Tasks;
using Android.Views;
using Android.Runtime;

namespace SSOTestApp2
{
    [Activity(Label = "SSOTestApp2", MainLauncher = true)]
    public class MainActivity2 : Activity
    {

        private static string STATE_DIALOG = "state_dialog";
        private static string STATE_INVALIDATE = "state_invalidate";
        private string TAG = "SSOTestApp2";
        private AccountManager mAccountManager;
        private AlertDialog mAlertDialog;
        private bool mInvalidate;
        private Account[] availableAccounts;
        private string tokenType;
        private string mAccountType;
        private string mAuthToken;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.Main);

                mAccountManager = AccountManager.Get(this);

                AddNewAccount(AccountGeneral.ACCOUNT_TYPE, AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS);
            }
            catch(System.Exception ex)
            {

            }
        }

        private void AddNewAccount(string accountType, string authTokenType)
        {
            try
            {
                this.mAccountType = accountType;
                this.mAuthToken = authTokenType;
                var thread = new Thread(() =>
                {

                    var future = mAccountManager.AddAccount(mAccountType, mAccountType, null, null, null, null, null);
                    var bundle = future.Result as Bundle;
                    if (bundle != null)
                    {
                        var intent = bundle.GetParcelable(AccountManager.KeyIntent) as Intent;
                        StartActivityForResult(intent, 1);
                    }

                    availableAccounts = mAccountManager.GetAccountsByType(AccountGeneral.ACCOUNT_TYPE);

                });
                thread.Start();
            }
            catch(System.Exception xe)
            {

            }
            
        }
    }
}

