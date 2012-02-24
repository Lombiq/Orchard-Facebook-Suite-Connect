using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Environment.Extensions;
using Piedone.Facebook.Suite.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Security;
using Piedone.Facebook.Suite.Services;
using Orchard.ContentManagement;
using Orchard.Settings;
using Orchard.UI.Notify;
using Orchard.Localization;

namespace Piedone.Facebook.Suite.Drivers
{
    [OrchardFeature("Piedone.Facebook.Suite.Connect")]
    public class FacebookConnectWidgetPartDriver : ContentPartDriver<FacebookConnectWidgetPart>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IFacebookConnectService _facebookConnectService;
        private readonly ISiteService _siteService;
        private readonly IFacebookSuiteService _facebookSuiteService;
        private readonly INotifier _notifier;

        public Localizer T { get; set; }

        public FacebookConnectWidgetPartDriver(
            IAuthenticationService authenticationService,
            IFacebookConnectService facebookConnectService,
            ISiteService siteService,
            IFacebookSuiteService facebookSuiteService,
            INotifier notifier)
        {
            _authenticationService = authenticationService;
            _facebookConnectService = facebookConnectService;
            _siteService = siteService;
            _facebookSuiteService = facebookSuiteService;
            _notifier = notifier;

            T = NullLocalizer.Instance;
        }


        protected override DriverResult Display(FacebookConnectWidgetPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_FacebookConnectWidget",
                () =>
                {
                    var settings = _siteService.GetSiteSettings().As<FacebookConnectSettingsPart>();

                    var isAuthenticated = _authenticationService.IsAuthenticated();
                    var isConnected = _facebookConnectService.AuthenticatedFacebookUserIsSaved();

                    IFacebookUser authenticatedFacebookUser = null;

                    if (!isAuthenticated && isConnected && settings.AutoLogin)
                    {
                        authenticatedFacebookUser = _facebookConnectService.UpdateAuthenticatedFacebookUser();

                        IEnumerable<FacebookConnectValidationKey> errors;
                        if (_facebookConnectService.UserIsValid(authenticatedFacebookUser, settings, out errors))
                        {
                            _authenticationService.SignIn(authenticatedFacebookUser.As<IUser>(), false);
                            isAuthenticated = true;
                            //CurrentUser.PictureLink = !String.IsNullOrEmpty(avatar.ImageUrl) ? avatar.ImageUrl : currentFacebookUserPart.PictureLink; 
                        }
                    }
                    else if (isConnected)
                    {
                        authenticatedFacebookUser = _facebookConnectService.GetAuthenticatedFacebookUser();
                    }

                    return shapeHelper.Parts_FacebookConnectWidget(
                                IsAuthenticated: isAuthenticated,
                                IsConnected: isConnected,
                                AuthenticatedFacebookUser: authenticatedFacebookUser);
                });
        }

        protected override DriverResult Editor(FacebookConnectWidgetPart part, dynamic shapeHelper)
        {
            if (!_facebookSuiteService.AppSettingsSet)
            {
                _notifier.Add(NotifyType.Error, T("Currently the Facebook app settings are not set, therefore Facebook Connect won't work properly. Please set the app settings first on the page Facebook Suite Settings under Settings."));
            }

            return null;
        }
    }
}