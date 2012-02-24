using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Mvc.Extensions;
using Orchard.Security;
using Orchard.Settings;
using Orchard.Themes;
using Orchard.UI.Notify;
using Orchard.Users.Services;
using Piedone.Facebook.Suite.Models;
using Piedone.Facebook.Suite.Services;

namespace Piedone.Facebook.Suite.Controllers
{
    [HandleError]
    [Themed]
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class ConnectController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ISiteService _siteService;
        private readonly IFacebookConnectService _facebookConnectService;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMembershipService _membershipService;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public ConnectController(
            IOrchardServices orchardServices,
            ISiteService siteService,
            IFacebookConnectService facebookConnectService,
            IUserService userService,
            IAuthenticationService authenticationService,
            IMembershipService membershipService,
            INotifier notifier)
        {
            _orchardServices = orchardServices;
            _siteService = siteService;
            _facebookConnectService = facebookConnectService;
            _userService = userService;
            _authenticationService = authenticationService;
            _membershipService = membershipService;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }

        public ActionResult Connect(string returnUrl = "")
        {
            var settings = GetSettings();
            var facebookUser = _facebookConnectService.FetchMe();

            if (ValidateUser(facebookUser))
            {
                if (_facebookConnectService.AuthenticatedFacebookUserIsSaved())
                {
                    var user = _facebookConnectService.UpdateAuthenticatedFacebookUser(facebookUser);
                    _authenticationService.SignIn(user.As<IUser>(), false);
                }
                // With this existing users can attach their FB account to their local accounts
                else if (_authenticationService.IsAuthenticated())
                {
                    _facebookConnectService.UpdateFacebookUser(_authenticationService.GetAuthenticatedUser(), facebookUser);
                }
                else if (settings.SimpleRegistration)
                {
                    return new ShapeResult(this, _orchardServices.New.FacebookConnectRegistrationChooser(ReturnUrl: returnUrl));
                }
                else return this.RedirectLocal(Url.Action("Register", "Account", new { Area = "Orchard.Users" }));
            }

            return this.RedirectLocal(returnUrl); // "this" is necessary, as this is from an extension (Orchard.Mvc.Extensions.ControllerExtensions)
        }

        public ActionResult SimpleRegistration(string returnUrl = "")
        {
            return SimpleRegistrationForm();
        }

        [HttpPost]
        public ActionResult SimpleRegistration(string userName, string returnUrl = "")
        {
            var settings = GetSettings();

            if (!settings.SimpleRegistration)
            {
                // Only adventurers who experiemnt will ever see this.
                _notifier.Error(T("Simple registration is not allowed on this site"));
                return this.RedirectLocal(returnUrl);
            }

            // This a notably elegant solution for not checking the e-mail :-). We don't require e-mail with simple registrations.
            if (!_userService.VerifyUserUnicity(userName, "dféfdéfdkék342ü45ü43ü453578"))
            {
                // Notifier or validation summary. 
                _notifier.Error(T("The username you tried to register is taken. Please choose another one."));
                //ModelState.AddModelError("userExists", T("The username you tried to register is taken. Please choose another one."));
                return SimpleRegistrationForm();
            }

            var facebookUser = _facebookConnectService.FetchMe();

            if (!ValidateUser(facebookUser)) return SimpleRegistrationForm();

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

        private ShapeResult SimpleRegistrationForm()
        {
            return new ShapeResult(this, _orchardServices.New.FacebookConnectSimpleRegistration());
        }

        private FacebookConnectSettingsPart GetSettings()
        {
            return _siteService.GetSiteSettings().As<FacebookConnectSettingsPart>();
        }

        private bool ValidateUser(IFacebookUser facebookUser)
        {
            IEnumerable<FacebookConnectValidationKey> errors;
            if (!_facebookConnectService.UserIsValid(facebookUser, GetSettings(), out errors))
            {
                foreach (var error in errors)
                {
                    switch (error)
                    {
                        case FacebookConnectValidationKey.NoPermissionsGranted:
                            _notifier.Error(T("You haven't granted the requested permissions."));
                            //ModelState.AddModelError("notVerified", T("You haven't granted the requested permissions."));
                            break;
                        case FacebookConnectValidationKey.NotVerified:
                            _notifier.Error(T("You're not a verified Facebook user. Only verified users are allowed to register, so please verify your account."));
                            //ModelState.AddModelError("notVerified", T("You're not a verified Facebook user. Only verified users are allowed to register, so please verify your account."));
                            break;
                    }
                }

                return false;
            }

            return true;
        }
    }
}
