using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Notify;
using Piedone.Facebook.Suite.Models;
using Piedone.Facebook.Suite.Services;

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

                    var authenticatedUser = _authenticationService.GetAuthenticatedUser();
                    var isConnected = _facebookConnectService.AuthenticatedFacebookUserIsSaved()
                        || (authenticatedUser != null && !string.IsNullOrEmpty(authenticatedUser.As<FacebookUserPart>().Name));

                    IFacebookUser authenticatedFacebookUser = null;

                    if (isConnected)
                    {
                        authenticatedFacebookUser = authenticatedUser.As<FacebookUserPart>();
                    }

                    return shapeHelper.Parts_FacebookConnectWidget(
                                IsAuthenticated: authenticatedUser != null,
                                IsConnected: isConnected,
                                IsAuthenticatedWithFacebookConnect: _facebookConnectService.IsAuthenticated(),
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