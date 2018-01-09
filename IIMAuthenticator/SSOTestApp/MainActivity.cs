using Android.App;
using Android.Widget;
using Android.OS;
using Android.Accounts;
using IIMAuthenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;

namespace SSOTestApp
{
    [Activity(Label = "SSOTestApp", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private static string STATE_DIALOG = "state_dialog";
	    private static string STATE_INVALIDATE = "state_invalidate";
        private string TAG = "SSOTestApp";
        private AccountManager mAccountManager;
        private AlertDialog mAlertDialog;
        private bool mInvalidate;
        private Account[] availableAccounts;
        private string tokenType;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            mAccountManager = AccountManager.Get(this);
            AddNewAccount(AccountGeneral.ACCOUNT_TYPE, AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS);
            ShowAccountPicker(AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS, true);
        }

        private void ShowAccountPicker(string authTokenType, bool invalidate)
        {
            mInvalidate = invalidate;
            tokenType = authTokenType;
            availableAccounts = mAccountManager.GetAccountsByType(AccountGeneral.ACCOUNT_TYPE);

            if (availableAccounts.Length == 0)
            {
                Toast.MakeText(this, "No accounts", ToastLength.Short).Show();
            }
            else
            {
                List<string> name = new List<string>();
                foreach(var acc in availableAccounts)
                {
                    name.Add(acc.Name);
                }


                // Account picker
                mAlertDialog = new AlertDialog.Builder(this).Create();
                mAlertDialog.SetTitle("Pick Account");
                mAlertDialog.ListView.Adapter = new ArrayAdapter<string>(BaseContext, Android.Resource.Layout.SimpleListItem1, name);
                mAlertDialog.ListView.ItemClick += ListView_ItemClick;
                //            SetAdapter(new ArrayAdapter<string>(BaseContext, Android.Resource.Layout.SimpleListItem1, name));
                //                {
                //            @Override
                //            public void onClick(DialogInterface dialog, int which)
                //    {
                //        if (invalidate)
                //            invalidateAuthToken(availableAccounts[which], authTokenType);
                //        else
                //            getExistingAccountAuthToken(availableAccounts[which], authTokenType);
                //    }
                //}).create();
                mAlertDialog.Show();
        }
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //if (mInvalidate)
            //    InvalidateAuthToken(availableAccounts[e.Position], tokenType);
            //else
            //    GetExistingAccountAuthToken(availableAccounts[e.Position], tokenType);
        }

        private void AddNewAccount(string accountType, string authTokenType)
        {
            IAccountManagerCallback callback = new AccountManagerCallback();
        }
        private class AccountManagerCallback : Java.Lang.Object, IAccountManagerCallback
        {
            void IAccountManagerCallback.Run(IAccountManagerFuture future)
            {
                if (future.IsCancelled)
                {
                    //task was cancelled code
                }
                else if (future.IsDone)
                {
                    //task is completed
                    Java.Lang.Object result = future.Result;
                    //ShowMessage("Account was created");
                    //process result
                }
            }

          
        }
        public void ShowMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;

          
        }

    }
}

