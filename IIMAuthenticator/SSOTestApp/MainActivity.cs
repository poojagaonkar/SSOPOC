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

                var arrAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, name);
                // Account picker
                var mAlertDialogBuilder = new AlertDialog.Builder(this);
                mAlertDialogBuilder.SetTitle("Pick Account");
                mAlertDialogBuilder.SetNegativeButton("Cancel", delegate { });
               
                mAlertDialogBuilder.SetAdapter(arrAdapter, new EventHandler<DialogClickEventArgs>((sender, args) =>{

                    if (invalidate)
                    {
                        InvalidateAuthToken(availableAccounts.ElementAt(args.Which), authTokenType);
                    }
                    else
                    {
                        GetExistingAccountAuthToken(availableAccounts.ElementAt(args.Which), tokenType);
                    }

                }));
              
                mAlertDialogBuilder.Create().Show();
        }
        }

       

        private void GetExistingAccountAuthToken(Account account, string tokenType)
        {
            var mFuture = mAccountManager.GetAuthToken(account, tokenType, null, null, null, null);
            var thread = new Thread(()=> {

                Bundle bnd = mFuture.Result as Bundle;
                string authtoken = bnd.GetString(AccountManager.KeyAuthtoken);

            });
            thread.Start();
        }

        private void InvalidateAuthToken(Account account, string authTokenType)
        {
            var mFuture = mAccountManager.GetAuthToken(account, tokenType, null, null, null, null);
            var thread = new Thread(() => {

                Bundle bnd = mFuture.Result as Bundle;
                string authtoken = bnd.GetString(AccountManager.KeyAuthtoken);
                mAccountManager.InvalidateAuthToken(account.Type, authtoken);
                ShowMessage(account.Name + ": Invalidated");
            });
            thread.Start();
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (mInvalidate)
                InvalidateAuthToken(availableAccounts[e.Position], tokenType);
            else
                GetExistingAccountAuthToken(availableAccounts[e.Position], tokenType);
        }

            /**
         * Add new account to the account manager
         * @param accountType
         * @param authTokenType
         */

        private void AddNewAccount(string accountType, string authTokenType)
        {
      
            this.mAccountType = accountType;
            this.mAuthToken = authTokenType;
            var thread = new Thread(NewAccount);
            thread.Start();
            //CheckIfFirstRun();
            //Finish();

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
       
        public void ShowMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            else
            {
                RunOnUiThread(() => {

                    Toast.MakeText(this, msg, ToastLength.Long).Show();

                });
            }
          
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

