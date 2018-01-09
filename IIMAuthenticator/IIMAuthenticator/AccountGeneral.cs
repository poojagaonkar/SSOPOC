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

namespace IIMAuthenticator
{
    public static class AccountGeneral
    {
        public static  string ACCOUNT_TYPE = "com.rapidcricle.iimauthenticatorapp";

        /**
         * Account name
         */
        public static  string ACCOUNT_NAME = "RapidCircle";

        /**
         * Auth token types
         */
        public static  String AUTHTOKEN_TYPE_READ_ONLY = "Read only";
        public static  String AUTHTOKEN_TYPE_READ_ONLY_LABEL = "Read only access to an Udinic account";

        public static  String AUTHTOKEN_TYPE_FULL_ACCESS = "Full access";
        public static  String AUTHTOKEN_TYPE_FULL_ACCESS_LABEL = "Full access to an Udinic account";

        public static  IServerAuthenticate sServerAuthenticate = new ParseComServerAuthenticate();
    }
}