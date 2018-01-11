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
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;

namespace com.rapidcircle.iimAuthenticator
{
    public interface IServerAuthenticate
    {
       // string UserSignUp(string name, string email, string pass, string authType);
        Task <AuthContainer> UserSignIn(string resource, IPlatformParameters platformParameters);
    }
}