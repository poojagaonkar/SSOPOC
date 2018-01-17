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

namespace SSOTestApp
{
    [Activity(Label = "SSOTestApp", MainLauncher = true)]
    public class MainActivity : Activity//, IAccountManagerFuture
    {
        private static string STATE_DIALOG = "state_dialog";
	    private static string STATE_INVALIDATE = "state_invalidate";
        private string TAG = "SSOTestApp";
        private AccountManager mAccountManager;
        private AlertDialog mAlertDialog;
        private bool mInvalidate;
        private Account[] availableAccounts;
        private string tokenType;
        private string mAccountType;
        private string mAuthToken;

        public bool IsCancelled
        {
            get
            {
                return false;
                //throw new NotImplementedException();
            }
        }

        public bool IsDone
        {
            get
            {
                return true;
                //throw new NotImplementedException();
            }
        }

        public Java.Lang.Object Result
        {
            get
            {
                Java.Lang.Object mObj = new Bundle();
                return mObj;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button btnAddAccount = FindViewById<Button>(Resource.Id.btnAddAccount);
            Button btnGetAuthToken = FindViewById<Button>(Resource.Id.btnGetAuthToken);
            Button btnInvalidateAuthToken = FindViewById<Button>(Resource.Id.btnInvalidateAuthToken);
            Button btnGetAuthTokenConvenient = FindViewById<Button>(Resource.Id.btnGetAuthTokenConvenient);

            mAccountManager = AccountManager.Get(this);

            btnAddAccount.Click +=  delegate {

                 AddNewAccount(AccountGeneral.ACCOUNT_TYPE, AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS);

                availableAccounts = mAccountManager.GetAccountsByType(AccountGeneral.ACCOUNT_TYPE);

            };

            btnGetAuthToken.Click += delegate {

                ShowAccountPicker(AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS, true);

            };
            btnGetAuthTokenConvenient.Click += delegate {

                GetTokenForAccountCreateIfNeeded(AccountGeneral.ACCOUNT_TYPE, AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS);

            };
            btnInvalidateAuthToken.Click += delegate {

               ShowAccountPicker(AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS, true);

            };

            if (savedInstanceState != null)
            {
                bool showDialog = savedInstanceState.GetBoolean(STATE_DIALOG);
                bool invalidate = savedInstanceState.GetBoolean(STATE_INVALIDATE);
                if (showDialog)
                {
                    ShowAccountPicker(AccountGeneral.AUTHTOKEN_TYPE_FULL_ACCESS, invalidate);
                }
            }
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (mAlertDialog != null && mAlertDialog.IsShowing)
            {
                outState.PutBoolean(STATE_DIALOG, true);
                outState.PutBoolean(STATE_INVALIDATE, mInvalidate);
            }
        }
        private void GetTokenForAccountCreateIfNeeded(string aCCOUNT_TYPE, string aUTHTOKEN_TYPE_FULL_ACCESS)
        {
            //throw new NotImplementedException();
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

            /**
         * Add new account to the account manager
         * @param accountType
         * @param authTokenType
         */

        private void AddNewAccount(string accountType, string authTokenType)
        {
            //IAccountManagerCallback callback = new AccountManagerCallback();
            //callback.Run(this);
            this.mAccountType = accountType;
            this.mAuthToken = authTokenType;
            var thread = new Thread(NewAccount);
            thread.Start();
            //CheckIfFirstRun();
            //Finish();

            var mFuture = mAccountManager.AddAccount(accountType, authTokenType, null, null, this, null, null);
        }

        private void NewAccount()
        {
            var future = mAccountManager.AddAccount(mAccountType,mAccountType, null, null, null, null, null);
            var bundle = future.Result as Bundle;
                if (bundle != null)
                {
                    var intent = bundle.GetParcelable(AccountManager.KeyIntent) as Intent;
                    StartActivityForResult(intent, 1);
                }
           
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }
        private class AccountManagerCallback : Java.Lang.Object, IAccountManagerCallback
        {
            void IAccountManagerCallback.Run(IAccountManagerFuture future)
            {
                try
                {
                    if (future.IsCancelled)
                    {
                        //task was cancelled code
                    }
                    else if (future.IsDone)
                    {
                        //task is completed
                        var result = future.Result as Bundle;
                        //ShowMessage("Account was created");
                        Log.Debug("IIM", "AddNewAccount Bundle is " + result);
                        //process result
                    }
                    

                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    //showMessage(e.getMessage());
                }
              
            }

          
        }
        public void ShowMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;

          
        }

        public bool Cancel(bool mayInterruptIfRunning)
        {
            throw new NotImplementedException();
        }

        public Java.Lang.Object GetResult(long timeout, TimeUnit unit)
        {
            throw new NotImplementedException();
        }
    }
}

