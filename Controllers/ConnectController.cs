using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.UI.Notify;
using Piedone.Facebook.Suite.Models;
using Piedone.Facebook.Suite.Services;
using Orchard;
using Orchard.Mvc;
using Orchard.Users.Services;
using System;
using Orchard.Security;
using Orchard.Themes;

namespace Piedone.Facebook.Suite.Controllers
{
    [HandleError]
    [Themed]
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class ConnectController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IFacebookConnectService _facebookConnectService;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public ConnectController(
            IOrchardServices orchardServices,
            IFacebookConnectService facebookConnectService,
            IUserService userService,
            IAuthenticationService authenticationService, 
            IMembershipService membershipService,
            INotifier notifier)
        {
            _orchardServices = orchardServices;
            _facebookConnectService = facebookConnectService;
            _userService = userService;
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        public ActionResult Connect(int connectId, string returnUrl = "")
        {
            if (IsAuthorized(connectId))
            {
                if (_facebookConnectService.AuthenticatedFacebookUserIsSaved())
                {
                    var facebookUser = _facebookConnectService.UpdateAuthenticatedFacebookUser();
                    _authenticationService.SignIn(facebookUser.As<IUser>(), false);
                }
                // With this existing users can attach their FB account to their local accounts
                else if (_authenticationService.IsAuthenticated())
                {
                    _facebookConnectService.UpdateFacebookUser(_authenticationService.GetAuthenticatedUser(), _facebookConnectService.FetchMe());
                }
                else return RegistrationForm();
            }
            else _notifier.Error(T("You're not logged in at Facebook or you haven't granted the requested permissions. Therefore, we were unable to log you in."));


            return this.RedirectLocal(returnUrl); // "this" is necessary, as this is from an extension (Orchard.Mvc.Extensions.ControllerExtensions)
        }

        [HttpPost]
        public ActionResult Connect(string userName, int connectId, string returnUrl = "")
        {
            // This a notably elegant solution for not checking the e-mail :-). We don't require e-mail from Facebook users currently.
            if (!_userService.VerifyUserUnicity(userName, "dféfdéfdkék342ü45ü43ü453578"))
            {
                // Notifier or validation summary. 
                _notifier.Error(T("The username you tried to register is taken. Please choose another one."));
                //ModelState.AddModelError("userExists", T("The username you tried to register is taken. Please choose another one."));
                return RegistrationForm();
            }

            var settings = GetSettings(connectId);

            var facebookUser = _facebookConnectService.FetchMe();

            if (settings.OnlyAllowVerified && !facebookUser.IsVerified)
            {
                _notifier.Error(T("You're not a verified Facebook user. Only verified users are allowed to register, so please verify your account."));
                //ModelState.AddModelError("notVerified", T("You're not a verified Facebook user. Only verified users are allowed to register, so please verify your account."));
                return RegistrationForm();
            }

            var random = new Random();
            var user = _membershipService.CreateUser(
                new CreateUserParams(
                    userName,
                    random.Next().ToString(),
                    "",
                    "",
                    "",
                    true));

            _facebookConnectService.UpdateFacebookUser(user, facebookUser);

            _authenticationService.SignIn(user, false);

            return this.RedirectLocal(returnUrl);
        }

        private ShapeResult RegistrationForm()
        {
            return new ShapeResult(this, _orchardServices.New.FacebookConnectRegistration());
        }

        private bool IsAuthorized(int connectId)
        {
            return _facebookConnectService.IsAuthorized(GetSettings(connectId).Permissions);
        }

        private FacebookConnectPart GetSettings(int connectId)
        {
            return _orchardServices.ContentManager.Get<FacebookConnectPart>(connectId);
        }
    }
}
