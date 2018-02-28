using Foundation;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using UIKit;

namespace IIMIosAuthenticator
{
    public partial class LoginViewController : UIViewController
    {
        public LoginViewController (IntPtr handle) : base (handle)
        {
        }
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();
            var result = await ADAuthService.Instance.Authenticate("https://graph.microsoft.com", new PlatformParameters(this));

        }
    }
}