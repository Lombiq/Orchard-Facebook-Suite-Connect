using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Extensions;
using Piedone.Facebook.Suite.Helpers;
using Piedone.Facebook.Suite.Services;

namespace Piedone.Facebook.Suite.Controllers
{
    [HandleError]
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class ConnectController : Controller
    {
        private readonly IFacebookConnectService _facebookConnectService;

        public ConnectController(IFacebookConnectService facebookConnectService)
        {
            _facebookConnectService = facebookConnectService;
        }

        /// <summary>
        /// Used when connecting through button click (not autologin)
        /// </summary>
        public ActionResult Connect(string permissions = "", bool onlyAllowVerified = false, string returnUrl = "")
        {
            _facebookConnectService.Authorize(
                permissions: FacebookConnectHelper.PermissionSettingsToArray(permissions), 
                onlyAllowVerified: onlyAllowVerified);

            return this.RedirectLocal(returnUrl); // this necessary, as this is from an extension (Orchard.Mvc.Extensions.ControllerExtensions)
        }
    }
}
