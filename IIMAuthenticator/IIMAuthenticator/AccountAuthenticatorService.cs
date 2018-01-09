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

namespace IIMAuthenticator
{
    [Service]
    [IntentFilter(new[] { "android.accounts.AccountAuthenticator" })]
    [MetaData("android.accounts.AccountAuthenticator", Resource = "@xml/authenticator")]
    public class AccountAuthenticatorService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            IIIMMainAuthenticator authenticator = new IIIMMainAuthenticator(this);
            return authenticator.IBinder;
        }
    }
}