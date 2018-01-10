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
using IIMAuthenticator;
using Android.Accounts;

namespace IIMAuthenticator
{
    [Service]
    [IntentFilter(new[] { "android.accounts.AccountAuthenticator" })]
    [MetaData("android.accounts.AccountAuthenticator", Resource = "@xml/authenticator")]
    public class AccountAuthenticatorService : Service
    {
        private static IIIMMainAuthenticator sAccountAuthenticator = null;

        public static string ACCOUNT_TYPE = "com.myapp.account";
        public static string ACCOUNT_NAME = "MyApp";
        private Context _context;

        public AccountAuthenticatorService() : base()
        {
            Console.WriteLine("Default Contructor");
        }
        public AccountAuthenticatorService(Context _context) : base()
        {
            Console.WriteLine("Secondary Constructor");
            this._context = _context;
        }
        public override IBinder OnBind(Intent intent)
        {
            //IIIMMainAuthenticator authenticator = new IIIMMainAuthenticator(this);
            //return authenticator.IBinder;

            Console.WriteLine("OnBind");
            IBinder ret = null;
            if (intent.Action == Android.Accounts.AccountManager.ActionAuthenticatorIntent)
                ret = GetAuthenticator().IBinder;
            return ret;
        }

        private IIIMMainAuthenticator GetAuthenticator()
        {
            Console.WriteLine("getAuthenticator");
            if (sAccountAuthenticator == null)
                sAccountAuthenticator = new IIIMMainAuthenticator(this);
            return sAccountAuthenticator;   
        }
    }
}