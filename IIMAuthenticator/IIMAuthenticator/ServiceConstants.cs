using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;

namespace com.rapidcircle.iimAuthenticator
{
    public static class ServiceConstants
    {
        public static string AUTHORITY = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";

        //public static string AUTHORITY = "https://login.windows.net/common";
        public static Uri RETURNURI = new Uri("http://checkin365-redirect");

        public static string CLIENTID = "a6bdca5c-0084-46fd-912d-8db9f74e31c3";
        public static string SHAREPOINTURL = null;
        public static string GRAPHURI = "https://graph.microsoft.com";
        public static string OFFICE365URI = "https://outlook.office365.com";
        public static string GraphToken { get; set; }

        public static string SITE_URL { get; set; }
        public static string SP_Token { get; set; }

        public static AuthenticationContext authContext { get; set; }
        public static string UserEmail { get; set; }
    }
}